using EduStream.Models;
using EduStream.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStream.Controllers
{
    [Authorize(Roles = "Administrador")] // Solo el administrador gestiona las categoria
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepositorio _categoriaRepo;

        public CategoriaController(ICategoriaRepositorio categoriaRepo)
        {
            _categoriaRepo = categoriaRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
        // lee
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var categorias = await _categoriaRepo.ObtenerTodasAsync();
            return Json(categorias);
        }

        // crea
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Categoria categoria)
        {
            if (!ModelState.IsValid) return BadRequest("Datos invalidos");

            categoria.Estado = true; 
            
            await _categoriaRepo.CrearAsync(categoria);
            return Json(new { success = true, message = "Categoria creada con exito", data = categoria });
        }

        // actualiza
        [HttpPost]
        public async Task<IActionResult> Editar([FromBody] Categoria categoria)
        {
            if (!ModelState.IsValid) return BadRequest("Datos invalidos");

            await _categoriaRepo.ActualizarAsync(categoria);
            return Json(new { success = true, message = "Categoria actualizada" });
        }

        // delete
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _categoriaRepo.EliminarAsync(id);
            return Json(new { success = true, message = "Categoria eliminada" });
        }
    }
}