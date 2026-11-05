using EduStream.Models;
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
        private readonly IRepositorioInscripcion _inscripcionRepo;
        private readonly ICursoRepositorio _cursoRepo; // 1. AGREGAMOS EL REPOSITORIO DE CURSOS

        // 2. LO INYECTAMOS EN EL CONSTRUCTOR
        public ProgresoLeccionController(
            IRepositorioProgresoLeccion progresoRepo,
            IRepositorioInscripcion inscripcionRepo,
            ICursoRepositorio cursoRepo)
        {
            _progresoRepo = progresoRepo;
            _inscripcionRepo = inscripcionRepo;
            _cursoRepo = cursoRepo;
        }

        public async Task<IActionResult> VerProgreso(int inscripcionId)
        {
            var inscripcion = await _inscripcionRepo.ObtenerPorIdAsync(inscripcionId);
            if (inscripcion == null) return NotFound();

            if (inscripcion.Estado == "Anulada" && !User.IsInRole("Administrador"))
            {
                TempData["Error"] = "Tu inscripción a este curso ha sido anulada. Perdiste el acceso al Aula Virtual.";
                return RedirectToAction("MisInscripciones", "Inscripcion");
            }

            if (!await ValidarPropiedadInscripcion(inscripcionId)) return Unauthorized();

            // 3. BUSCAMOS EL CURSO CON TODAS SUS LECCIONES Y MATERIALES
            var curso = await _cursoRepo.ObtenerCursoConDetallesAsync(inscripcion.CursoId);
            if (curso == null) return NotFound();

            var progresosGuardados = await _progresoRepo.ObtenerPorInscripcionIdAsync(inscripcionId);
            var progresosCompletos = curso.Lecciones.Select(leccion =>
            {
                var progresoExistente = progresosGuardados.FirstOrDefault(p => p.LeccionId == leccion.Id);

                return new ProgresoLeccion
                {
                    LeccionId = leccion.Id,
                    Leccion = leccion,
                    InscripcionId = inscripcionId,
                    Completada = progresoExistente != null && progresoExistente.Completada
                };
            }).ToList();

            ViewBag.InscripcionId = inscripcionId;
            ViewBag.TotalCompletadas = progresosGuardados.Count(p => p.Completada);

            //LISTA COMPLETA A LA VISTA
            return View(progresosCompletos);
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

            // 1. Guardamos el progreso en la BD
            bool resultado = await _progresoRepo.GuardarOActualizarProgresoAsync(
                request.InscripcionId,
                request.LeccionId,
                request.Completada
            );

            // 2. Calculamos los nuevos totales directamente aquí
            var inscripcion = await _inscripcionRepo.ObtenerPorIdAsync(request.InscripcionId);
            var curso = await _cursoRepo.ObtenerCursoConDetallesAsync(inscripcion.CursoId);
            var totalCompletadas = await _progresoRepo.ObtenerTotalLeccionesCompletadasAsync(request.InscripcionId);

            int totalLecciones = curso.Lecciones.Count;
            int porcentajeReal = totalLecciones > 0 ? (int)Math.Round((double)totalCompletadas / totalLecciones * 100) : 0;

            // 3. Devolvemos un JSON "plano" con los números exactos que espera el script de la vista
            return Json(new
            {
                success = resultado,
                porcentaje = porcentajeReal,
                completadas = totalCompletadas,
                totalLecciones = totalLecciones
            });
        }

        private async Task<bool> ValidarPropiedadInscripcion(int inscripcionId)
        {
            // admin ve todo
            if (User.IsInRole("Administrador")) return true;

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return false;

            var inscripcion = await _inscripcionRepo.ObtenerPorIdAsync(inscripcionId);

            return inscripcion != null
                && inscripcion.EstudianteId == userId
                && inscripcion.Estado != "Anulada";
        }

        public class ProgresoRequest
        {
            public int InscripcionId { get; set; }
            public int LeccionId { get; set; }
            public bool Completada { get; set; }
        }
    }
}