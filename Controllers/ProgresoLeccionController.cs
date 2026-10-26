using EduStream.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EduStream.Controllers
{
    public class ProgresoLeccionController : Controller
    {
        private readonly IRepositorioProgresoLeccion _progresoRepo;

        public ProgresoLeccionController(IRepositorioProgresoLeccion progresoRepo)
        {
            _progresoRepo = progresoRepo;
        }

        public async Task<IActionResult> VerProgreso(int inscripcionId)
        {
            var progresos = await _progresoRepo.ObtenerPorInscripcionIdAsync(inscripcionId);
            ViewBag.InscripcionId = inscripcionId;
            ViewBag.TotalCompletadas = await _progresoRepo.ObtenerTotalLeccionesCompletadasAsync(inscripcionId);

            return View(progresos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(int inscripcionId, int leccionId, bool completada)
        {
            bool exito = await _progresoRepo.GuardarOActualizarProgresoAsync(inscripcionId, leccionId, completada);

            if (!exito)
            {
                TempData["Error"] = "No se pudo actualizar el progreso de la lección.";
            }

            return RedirectToAction(nameof(VerProgreso), new { inscripcionId });
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarProgresoAjax([FromBody] ProgresoRequest request)
        {
            if (request == null) return BadRequest();

            bool resultado = await _progresoRepo.GuardarOActualizarProgresoAsync(
                request.InscripcionId, 
                request.LeccionId, 
                request.Completada
            );

            return Json(new { success = resultado });
        }
    }

    public class ProgresoRequest
    {
        public int InscripcionId { get; set; }
        public int LeccionId { get; set; }
        public bool Completada { get; set; }
    }
}