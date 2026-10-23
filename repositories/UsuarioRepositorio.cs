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
            // Buscamos al usuario solo por el email
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            // verificamos que la contraseña plana coincida con el Hash guardado
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
    }
}