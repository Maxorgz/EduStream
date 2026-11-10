using EduStream.Models;
using EduStream.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace EduStream.Controllers
{

    [Authorize]
    public class CursoController : Controller
    {
        private readonly ICursoRepositorio _cursoRepo;
        private readonly IUsuarioRepositorio _usuarioRepo;
        private readonly ICategoriaRepositorio _categoriaRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CursoController(ICursoRepositorio cursoRepo, IUsuarioRepositorio usuarioRepo, ICategoriaRepositorio categoriaRepo, IWebHostEnvironment webHostEnvironment)
        {
            _cursoRepo = cursoRepo;
            _usuarioRepo = usuarioRepo;
            _categoriaRepo = categoriaRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        // paginado en servidor y busqueda
        [Authorize(Roles = "Profesor,Administrador")]
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

            // Si esta inactivo, bloqueamos a los estudiantes
            if (curso.Estado == "Inactivo")
            {
                if (!User.IsInRole("Administrador") && !User.IsInRole("Profesor"))
                {
                    TempData["Error"] = "Este curso ya no está disponible.";
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(curso);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Catalogo(string? buscar, int pagina = 1)
        {
            int cantidadPorPagina = 6;

            var cursos = await _cursoRepo.ObtenerTodosAsync();
            var cursosActivos = cursos.Where(c => c.Estado == "Activo");

            if (!string.IsNullOrEmpty(buscar))
            {
                cursosActivos = cursosActivos.Where(c =>
                    c.Titulo.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    c.Descripcion.Contains(buscar, StringComparison.OrdinalIgnoreCase));
            }

            int totalRegistros = cursosActivos.Count();
            var cursosPaginados = cursosActivos
                                    .Skip((pagina - 1) * cantidadPorPagina)
                                    .Take(cantidadPorPagina)
                                    .ToList();

            ViewBag.BusquedaActual = buscar;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / cantidadPorPagina);

            return View(cursosPaginados);
        }

        // SEGURIDAD Y ROLES
        [Authorize(Roles = "Administrador,Profesor")]
        public async Task<IActionResult> Create()
        {
            await CargarCategoriasViewBagAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Profesor")]
        public async Task<IActionResult> Create(Curso curso)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int profesorId))
            {
                curso.ProfesorId = profesorId;
            }

            ModelState.Remove("Profesor");

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

            await CargarCategoriasViewBagAsync();
            return View(curso);
        }

        [Authorize(Roles = "Profesor,Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _cursoRepo.ObtenerPorIdAsync(id);
            if (curso == null) return NotFound();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int profesorId) && curso.ProfesorId != profesorId && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            await CargarCategoriasViewBagAsync();
            return View(curso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Profesor")]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.Id) return BadRequest();

            ModelState.Remove("Profesor");

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
            await CargarCategoriasViewBagAsync();
            return View(curso);
        }

        private async Task CargarCategoriasViewBagAsync()
        {
            var categorias = await _categoriaRepo.ObtenerTodasAsync();
            // SelectList(OrigenDeDatos, CampoValor, CampoTexto)
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre");
        }


        // busqueda AJAX
        [HttpGet]
        public async Task<IActionResult> BuscarProfesoresAjax(string termino)
        {
            var profesores = await _usuarioRepo.BuscarPorRolYNombreAsync("Profesor", termino);

            var resultados = profesores.Select(p => new
            {
                id = p.Id,
                text = p.Nombre + " " + p.Apellido
            });

            return Json(resultados);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Profesor")]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var exito = await _cursoRepo.CambiarEstadoAsync(id);

            if (exito)
                TempData["Exito"] = "El estado del curso se actualizó correctamente.";
            else
                TempData["Error"] = "No se pudo actualizar el estado del curso.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> BuscarCursosAjax(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
                return Json(new List<object>());

            var todos = await _cursoRepo.ObtenerTodosAsync();

            var filtrados = todos.Where(c =>
                (c.Titulo != null && c.Titulo.Contains(termino, StringComparison.OrdinalIgnoreCase)) &&
                c.Estado == "Activo"
            ).ToList();

            var resultados = filtrados.Select(c => new
            {
                id = c.Id,
                text = c.Titulo
            });

            return Json(resultados);
        }

    }
}