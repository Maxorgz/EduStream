using EduStream.Models;
using EduStream.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduStream.Controllers
{
    [Authorize]
    public class InscripcionController : Controller
    {
        private readonly IRepositorioInscripcion _inscripcionRepo;
        private readonly ICursoRepositorio _cursoRepo;

        public InscripcionController(IRepositorioInscripcion inscripcionRepo, ICursoRepositorio cursoRepo)
        {
            _inscripcionRepo = inscripcionRepo;
            _cursoRepo = cursoRepo;
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index(int pagina = 1)
        {
            int cantidadPorPagina = 10;
            var inscripciones = await _inscripcionRepo.ObtenerPaginadoAsync(pagina, cantidadPorPagina);
            var totalRegistros = await _inscripcionRepo.ContarTotalAsync();
            
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / cantidadPorPagina);

            return View(inscripciones);
        }

        [Authorize(Roles = "Estudiante")]
        public async Task<IActionResult> MisInscripciones()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int estudianteId)) return Unauthorized();

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
        [Authorize(Roles = "Estudiante")]
        public async Task<IActionResult> Inscribir(int cursoId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int estudianteId)) return Unauthorized();

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
                EstadoPago = "Aprobado" // O Pendiente
            };

            await _inscripcionRepo.CrearAsync(nuevaInscripcion);
            TempData["Exito"] = "Inscripción realizada con éxito.";

            return RedirectToAction("MisInscripciones");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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