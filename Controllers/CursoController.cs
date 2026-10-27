using EduStream.Models;
using EduStream.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStream.Controllers
{

    [Authorize]
    public class CursoController : Controller
    {
        private readonly ICursoRepositorio _cursoRepo;
        private readonly IUsuarioRepositorio _usuarioRepo; 
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CursoController(ICursoRepositorio cursoRepo, IUsuarioRepositorio usuarioRepo, IWebHostEnvironment webHostEnvironment)
        {
            _cursoRepo = cursoRepo;
            _usuarioRepo = usuarioRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        // paginado en servidor y busqueda
        [AllowAnonymous] 
        public async Task<IActionResult> Index(string? buscar, int pagina = 1)
        {
            int cantidadPorPagina = 10;
            var cursos = await _cursoRepo.ObtenerPaginadoAsync(buscar, pagina, cantidadPorPagina);
            var totalRegistros = await _cursoRepo.ContarTotalAsync(buscar);

            ViewBag.BusquedaActual = buscar;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / cantidadPorPagina);

            return View(cursos);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var curso = await _cursoRepo.ObtenerCursoConDetallesAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // SEGURIDAD Y ROLES
        [Authorize(Roles = "Administrador,Profesor")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Profesor")]
        public async Task<IActionResult> Create(Curso curso)
        {
            if (ModelState.IsValid)
            {
                if (curso.PortadaFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "portadas");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + curso.PortadaFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await curso.PortadaFile.CopyToAsync(fileStream);
                    }
                    curso.PortadaUrl = "/uploads/portadas/" + uniqueFileName;
                }

                await _cursoRepo.CrearAsync(curso);
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        [Authorize(Roles = "Administrador,Profesor")]
        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _cursoRepo.ObtenerPorIdAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Profesor")]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                if (curso.PortadaFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "portadas");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + curso.PortadaFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await curso.PortadaFile.CopyToAsync(fileStream);
                    }
                    curso.PortadaUrl = "/uploads/portadas/" + uniqueFileName;
                }

                await _cursoRepo.ActualizarAsync(curso);
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")] // Solo el admin borra cursos
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _cursoRepo.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // busqueda AJAX
        [HttpGet]
        public async Task<IActionResult> BuscarProfesoresAjax(string termino)
        {
            var profesores = await _usuarioRepo.BuscarPorRolYNombreAsync("Profesor", termino);
            
            var resultados = profesores.Select(p => new {
                id = p.Id,
                text = p.Nombre + " " + p.Apellido
            });

            return Json(resultados);
        }
    }
}