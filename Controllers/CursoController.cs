using EduStream.Models;
using EduStream.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduStream.Controllers
{
    public class CursoController : Controller
    {
        private readonly ICursoRepositorio _cursoRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CursoController(ICursoRepositorio cursoRepo, IWebHostEnvironment webHostEnvironment)
        {
            _cursoRepo = cursoRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string? buscar)
        {
            IEnumerable<Curso> cursos;
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                cursos = await _cursoRepo.BuscarAsync(buscar);
                ViewBag.BusquedaActual = buscar;
            }
            else
            {
                cursos = await _cursoRepo.ObtenerTodosAsync();
            }
            return View(cursos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var curso = await _cursoRepo.ObtenerCursoConDetallesAsync(id);
            if (curso == null) return NotFound();

            return View(curso);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _cursoRepo.ObtenerPorIdAsync(id);
            if (curso == null) return NotFound();

            return View(curso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _cursoRepo.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}