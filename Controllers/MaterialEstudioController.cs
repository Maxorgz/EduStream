using EduStream.Models;
using EduStream.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStream.Controllers
{
    [Authorize(Roles = "Administrador,Profesor")] 
    public class MaterialEstudioController : Controller
    {
        private readonly IMaterialEstudioRepositorio _materialRepo;
        private readonly ILeccionRepositorio _leccionRepo;
        private readonly IWebHostEnvironment _env;

        public MaterialEstudioController(IMaterialEstudioRepositorio materialRepo, ILeccionRepositorio leccionRepo, IWebHostEnvironment env)
        {
            _materialRepo = materialRepo;
            _leccionRepo = leccionRepo;
            _env = env;
        }

        //Redirigimos al indice de lecciones si intentan entrar directamente
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Curso");
        }

        public async Task<IActionResult> Crear(int leccionId)
        {
            var leccion = await _leccionRepo.ObtenerPorIdAsync(leccionId);
            if (leccion == null) return NotFound();

            ViewBag.LeccionTitulo = leccion.Titulo;
            ViewBag.CursoId = leccion.CursoId;

            var nuevoMaterial = new MaterialEstudio 
            { 
                LeccionId = leccionId 
            };

            return View(nuevoMaterial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(MaterialEstudio modelo)
        {
            if (ModelState.IsValid)
            {
                if (modelo.ArchivoFile != null)
                {
                    //Crear la carpeta si no existe
                    string carpetaDestino = Path.Combine(_env.WebRootPath, "uploads", "materiales");
                    if (!Directory.Exists(carpetaDestino))
                    {
                        Directory.CreateDirectory(carpetaDestino);
                    }

                    // Generar un nombre unico para no sobrescribir archivos con el mismo nombre
                    string extension = Path.GetExtension(modelo.ArchivoFile.FileName);
                    string nombreUnico = Guid.NewGuid().ToString() + extension;
                    string rutaCompleta = Path.Combine(carpetaDestino, nombreUnico);

                    //Guardar el archivo fisico en el servidor
                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await modelo.ArchivoFile.CopyToAsync(stream);
                    }

                    //Guardar la URL relativa en la base de datos
                    modelo.ArchivoUrl = "/uploads/materiales/" + nombreUnico;
                }
                else
                {
                    ModelState.AddModelError("ArchivoFile", "Debes adjuntar un archivo.");
                    return View(modelo);
                }

                modelo.FechaCreacion = DateTime.Now;
                await _materialRepo.CrearAsync(modelo);

                TempData["Exito"] = "Material subido correctamente.";
                
                // Redirigir de vuelta a los detalles del curso o leccion
                var leccion = await _leccionRepo.ObtenerPorIdAsync(modelo.LeccionId);
                return RedirectToAction("Details", "Curso", new { id = leccion.CursoId });
            }

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var material = await _materialRepo.ObtenerPorIdAsync(id);
            if (material == null) return NotFound();

            var leccion = await _leccionRepo.ObtenerPorIdAsync(material.LeccionId);

            // Borrar el archivo fisico del disco para ahorrar espacio
            if (!string.IsNullOrEmpty(material.ArchivoUrl))
            {
                string rutaFisica = Path.Combine(_env.WebRootPath, material.ArchivoUrl.TrimStart('/'));
                if (System.IO.File.Exists(rutaFisica))
                {
                    System.IO.File.Delete(rutaFisica);
                }
            }

            await _materialRepo.EliminarAsync(id);
            TempData["Exito"] = "Material eliminado correctamente.";

            return RedirectToAction("Details", "Curso", new { id = leccion.CursoId });
        }
        
        [Authorize(Roles="Estudiante,Profesor...")] // Permitimos a los alumnos descargar
        public async Task<IActionResult> Descargar(int id)
        {
            var material = await _materialRepo.ObtenerPorIdAsync(id);
            if (material == null || string.IsNullOrEmpty(material.ArchivoUrl)) return NotFound();

            string rutaFisica = Path.Combine(_env.WebRootPath, material.ArchivoUrl.TrimStart('/'));
            if (!System.IO.File.Exists(rutaFisica)) return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(rutaFisica);
            return File(bytes, "application/octet-stream", Path.GetFileName(rutaFisica));
        }
    }
}