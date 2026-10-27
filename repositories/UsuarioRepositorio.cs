using EduStream.Data;
using EduStream.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace EduStream.Repositories
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly EduStreamDbContext _context;

        public UsuarioRepositorio(EduStreamDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> ObtenerPorCredencialesAsync(string email, string password)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario != null && BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            {
                return usuario;
            }

            return null;
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task AgregarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Usuario>> BuscarPorRolYNombreAsync(string rol, string termino)
        {
            var query = _context.Usuarios.Where(u => u.Rol == rol).AsQueryable();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                query = query.Where(u => u.Nombre.Contains(termino) || u.Apellido.Contains(termino));
            }

            return await query.Take(20).ToListAsync();
        }

        // metodos para paginado del controlador

        public async Task<IEnumerable<Usuario>> ObtenerPaginadoAsync(string? searchTerm, int pagina, int cantidadPorPagina)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Nombre.Contains(searchTerm) || 
                                         u.Apellido.Contains(searchTerm) || 
                                         u.Email.Contains(searchTerm));
            }

            return await query
                .Skip((pagina - 1) * cantidadPorPagina)
                .Take(cantidadPorPagina)
                .ToListAsync();
        }

        public async Task<int> ContarTotalAsync(string? searchTerm)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Nombre.Contains(searchTerm) || 
                                         u.Apellido.Contains(searchTerm) || 
                                         u.Email.Contains(searchTerm));
            }

            return await query.CountAsync();
        }

        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            await AgregarAsync(usuario);
            return usuario;
        }
    }
}