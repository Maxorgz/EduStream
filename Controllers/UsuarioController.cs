using EduStream.Models;
using EduStream.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduStream.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsuarioController(IUsuarioRepositorio usuarioRepo, IWebHostEnvironment webHostEnvironment)
        {
            _usuarioRepo = usuarioRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        // autenticacion (LOGIN Y LOGOUT)

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Por favor, completa todos los campos.");
                return View();
            }

            var usuario = await _usuarioRepo.ObtenerPorEmailAsync(email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas. Verifica tu correo y contraseña.");
                return View();
            }

            if (!usuario.Activo)
            {
                ModelState.AddModelError(string.Empty, "Tu cuenta se encuentra inactiva. Contacta al administrador.");
                return View();
            }

            // Creamos las credenciales y claims para la Cookie de autenticación
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            TempData["Exito"] = $"¡Bienvenido de nuevo, {usuario.Nombre}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Registro de nuevos estudiantes

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario usuario, string passwordConfirmacion)
        {
            // Validar si el email ya existe
            var existe = await _usuarioRepo.ObtenerPorEmailAsync(usuario.Email);
            if (existe != null)
            {
                ModelState.AddModelError("Email", "Ya existe una cuenta registrada con este correo electrónico.");
            }

            if (usuario.PasswordHash != passwordConfirmacion)
            {
                ModelState.AddModelError("PasswordHash", "Las contraseñas no coinciden.");
            }

            if (ModelState.IsValid)
            {
                // aca Hasheamos la contraseña con BCrypt
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
                
                // Forzamos por seguridad los registros publicos de Estudiantes y los activos
                usuario.Rol = "Estudiante";
                usuario.Activo = true;
                usuario.FechaRegistro = DateTime.Now;

                await _usuarioRepo.CrearAsync(usuario);

                TempData["Exito"] = "Registro exitoso. Ahora puedes iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }

            return View(usuario);
        }

        // gestion de usuarios (administracion)

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index(string? buscar, int pagina = 1)
        {
            int cantidadPorPagina = 10;
            var usuarios = await _usuarioRepo.ObtenerPaginadoAsync(buscar, pagina, cantidadPorPagina);
            var totalRegistros = await _usuarioRepo.ContarTotalAsync(buscar);

            ViewBag.BusquedaActual = buscar;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / cantidadPorPagina);

            return View(usuarios);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int id)
        {
            var usuario = await _usuarioRepo.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var usuario = await _usuarioRepo.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();

            usuario.Activo = !usuario.Activo;
            await _usuarioRepo.ActualizarAsync(usuario);

            TempData["Exito"] = "El estado del usuario ha sido actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // perfil de usuario autenticado

        [Authorize]
        public async Task<IActionResult> Perfil()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var usuario = await _usuarioRepo.ObtenerPorIdAsync(userId);
            if (usuario == null) return NotFound();

            return View(usuario);
        }
    }
}