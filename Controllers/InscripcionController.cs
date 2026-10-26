using EduStream.Models;
using EduStream.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EduStream.Controllers
{
    public class InscripcionController : Controller
    {
        private readonly IRepositorioInscripcion _inscripcionRepo;
        private readonly ICursoRepositorio _cursoRepo;

        public InscripcionController(IRepositorioInscripcion inscripcionRepo, ICursoRepositorio cursoRepo)
        {
            _inscripcionRepo = inscripcionRepo;
            _cursoRepo = cursoRepo;
        }

        public async Task<IActionResult> Index()
        {
            var inscripciones = await _inscripcionRepo.ObtenerTodasAsync();
            return View(inscripciones);
        }

        public async Task<IActionResult> MisInscripciones(int estudianteId)
        {
            var misInscripciones = await _inscripcionRepo.ObtenerPorEstudianteIdAsync(estudianteId);
            return View(misInscripciones);
        }

        public async Task<IActionResult> Details(int id)
        {
            var inscripcion = await _inscripcionRepo.ObtenerPorIdAsync(id);
            if (inscripcion == null) return NotFound();

            return View(inscripcion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inscribir(int estudianteId, int cursoId)
        {
            bool yaExiste = await _inscripcionRepo.ExisteInscripcionAsync(estudianteId, cursoId);
            if (yaExiste)
            {
                TempData["Error"] = "Ya te encuentras inscripto en este curso.";
                return RedirectToAction("Details", "Curso", new { id = cursoId });
            }

            var curso = await _cursoRepo.ObtenerPorIdAsync(cursoId);
            if (curso == null) return NotFound();

            var nuevaInscripcion = new Inscripcion
            {
                EstudianteId = estudianteId,
                CursoId = cursoId,
                FechaInscripcion = DateTime.Now,
                Estado = "Activa",
                MontoPago = curso.Precio,
                EstadoPago = "Aprobado" 
            };

            await _inscripcionRepo.CrearAsync(nuevaInscripcion);
            TempData["Exito"] = "Inscripción realizada con éxito.";

            return RedirectToAction("MisInscripciones", new { estudianteId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoPago(int id, string nuevoEstadoPago)
        {
            var inscripcion = await _inscripcionRepo.ObtenerPorIdAsync(id);
            if (inscripcion == null) return NotFound();

            inscripcion.EstadoPago = nuevoEstadoPago;
            await _inscripcionRepo.ActualizarAsync(inscripcion);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Anular(int id)
        {
            var inscripcion = await _inscripcionRepo.ObtenerPorIdAsync(id);
            if (inscripcion == null) return NotFound();

            inscripcion.Estado = "Anulada";
            await _inscripcionRepo.ActualizarAsync(inscripcion);

            return RedirectToAction(nameof(Index));
        }
    }
}