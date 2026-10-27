using EduStream.Models;
using EduStream.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStream.Controllers
{
    [Authorize]
    public class LeccionController : Controller
    {
        private readonly ILeccionRepositorio _leccionRepo;
        private readonly ICursoRepositorio _cursoRepo;

        public LeccionController(ILeccionRepositorio leccionRepo, ICursoRepositorio cursoRepo)
        {
            _leccionRepo = leccionRepo;
            _cursoRepo = cursoRepo;
        }

        public async Task<IActionResult> PorCurso(int cursoId)
        {
            var curso = await _cursoRepo.ObtenerPorIdAsync(cursoId);
            if (curso == null) return NotFound();

            var lecciones = await _leccionRepo.ObtenerPorCursoIdAsync(cursoId);
            ViewBag.Curso = curso;

            return View(lecciones);
        }

        public async Task<IActionResult> Details(int id)
        {
            var leccion = await _leccionRepo.ObtenerLeccionConMaterialesAsync(id);
            if (leccion == null) return NotFound();

            return View(leccion);
        }

        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> Create(int cursoId)
        {
            var curso = await _cursoRepo.ObtenerPorIdAsync(cursoId);
            if (curso == null) return NotFound();

            var nuevaLeccion = new Leccion
            {
                CursoId = cursoId,
                Orden = 1
            };

            ViewBag.CursoTitulo = curso.Titulo;
            return View(nuevaLeccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> Create(Leccion leccion)
        {
            if (ModelState.IsValid)
            {
                await _leccionRepo.CrearAsync(leccion);
                TempData["Exito"] = "Lección creada correctamente.";
                return RedirectToAction(nameof(PorCurso), new { cursoId = leccion.CursoId });
            }

            var curso = await _cursoRepo.ObtenerPorIdAsync(leccion.CursoId);
            if (curso != null) ViewBag.CursoTitulo = curso.Titulo;

            return View(leccion);
        }

        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> Edit(int id)
        {
            var leccion = await _leccionRepo.ObtenerPorIdAsync(id);
            if (leccion == null) return NotFound();

            return View(leccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> Edit(int id, Leccion leccion)
        {
            if (id != leccion.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                bool exito = await _leccionRepo.ActualizarAsync(leccion);
                if (!exito) return NotFound();

                TempData["Exito"] = "Lección actualizada correctamente.";
                return RedirectToAction(nameof(PorCurso), new { cursoId = leccion.CursoId });
            }

            return View(leccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Profesor,Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var leccion = await _leccionRepo.ObtenerPorIdAsync(id);
            if (leccion == null) return NotFound();

            int cursoId = leccion.CursoId;
            await _leccionRepo.EliminarAsync(id);

            TempData["Exito"] = "Lección eliminada correctamente.";
            return RedirectToAction(nameof(PorCurso), new { cursoId });
        }
    }
}