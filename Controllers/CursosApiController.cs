using EduStream.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStream.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // usamos JWT y rechazamos Cookies web para esta API
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CursosApiController : ControllerBase
    {
        private readonly ICursoRepositorio _cursoRepo;

        public CursosApiController(ICursoRepositorio cursoRepo)
        {
            _cursoRepo = cursoRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetCursos()
        {
            // Traemos una lista de cursos
            var cursos = await _cursoRepo.ObtenerPaginadoAsync(null, 1, 50);

            // Mapeamos los datos para evitar ciclos infinitos en el JSON
            var resultados = cursos.Select(c => new
            {
                c.Id,
                c.Titulo,
                c.Descripcion,
                c.Precio,
                c.PortadaUrl,
                Profesor = c.Profesor != null ? $"{c.Profesor.Nombre} {c.Profesor.Apellido}" : "Sin asignar",
                Categoria = c.Categoria != null ? c.Categoria.Nombre : "Sin categoría"
            });

            return Ok(resultados);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurso(int id)
        {
            var curso = await _cursoRepo.ObtenerCursoConDetallesAsync(id);

            if (curso == null) 
            {
                return NotFound(new { mensaje = "Curso no encontrado" });
            }

            var resultado = new
            {
                curso.Id,
                curso.Titulo,
                curso.Descripcion,
                curso.Precio,
                curso.PortadaUrl,
                Profesor = curso.Profesor != null ? $"{curso.Profesor.Nombre} {curso.Profesor.Apellido}" : "Sin asignar",
                Categoria = curso.Categoria != null ? curso.Categoria.Nombre : "Sin categoría",
                //resumen de lecciones
                Lecciones = curso.Lecciones?.Select(l => new 
                { 
                    l.Id, 
                    l.Titulo, 
                    l.Orden 
                })
            };

            return Ok(resultado);
        }
    }
}