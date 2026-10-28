using EduStream.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduStream.Controllers
{
    [Authorize] 
    public class ProgresoLeccionController : Controller
    {
        private readonly IRepositorioProgresoLeccion _progresoRepo;
        private readonly IRepositorioInscripcion _inscripcionRepo; // Valida seguridad

        public ProgresoLeccionController(IRepositorioProgresoLeccion progresoRepo, IRepositorioInscripcion inscripcionRepo)
        {
            _progresoRepo = progresoRepo;
            _inscripcionRepo = inscripcionRepo;
        }

        public async Task<IActionResult> VerProgreso(int inscripcionId)
        {
            // Validacion de seguridad
            if (!await ValidarPropiedadInscripcion(inscripcionId)) return Unauthorized();

            var progresos = await _progresoRepo.ObtenerPorInscripcionIdAsync(inscripcionId);
            ViewBag.InscripcionId = inscripcionId;
            ViewBag.TotalCompletadas = await _progresoRepo.ObtenerTotalLeccionesCompletadasAsync(inscripcionId);

            return View(progresos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(int inscripcionId, int leccionId, bool completada)
        {
            if (!await ValidarPropiedadInscripcion(inscripcionId)) return Unauthorized();

            bool exito = await _progresoRepo.GuardarOActualizarProgresoAsync(inscripcionId, leccionId, completada);

            if (!exito)
            {
                TempData["Error"] = "No se pudo actualizar el progreso de la leccion.";
            }

            return RedirectToAction(nameof(VerProgreso), new { inscripcionId });
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarProgresoAjax([FromBody] ProgresoRequest request)
        {
            if (request == null) return BadRequest();

            // Seguridad en endpoint AJAX
            if (!await ValidarPropiedadInscripcion(request.InscripcionId)) 
                return Json(new { success = false, message = "Acceso denegado" });

            bool resultado = await _progresoRepo.GuardarOActualizarProgresoAsync(
                request.InscripcionId, 
                request.LeccionId, 
                request.Completada
            );

            return Json(new { success = resultado });
        }

        private async Task<bool> ValidarPropiedadInscripcion(int inscripcionId)
        {
            // admin ve todo
            if (User.IsInRole("Administrador")) return true;

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return false;

            var inscripcion = await _inscripcionRepo.ObtenerPorIdAsync(inscripcionId);
            
            // devuelve true solo si la inscripcion existe y pertenece al usuario actual
            return inscripcion != null && inscripcion.EstudianteId == userId;
        }
    }

    public class ProgresoRequest
    {
        public int InscripcionId { get; set; }
        public int LeccionId { get; set; }
        public bool Completada { get; set; }
    }
}