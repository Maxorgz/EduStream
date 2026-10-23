using EduStream.Data;
using EduStream.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStream.Repositories
{
    public class RepositorioCategoria : RepositorioBase, ICategoriaRepositorio
    {
        public RepositorioCategoria(EduStreamDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodasAsync()
        {
            return await _context.Categorias
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Categoria?> ObtenerConCursosAsync(int id)
        {
            return await _context.Categorias
                .Include(c => c.Cursos)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Categoria> CrearAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<bool> ActualizarAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            _context.Categorias.Remove(categoria);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExisteNombreAsync(string nombre)
        {
            return await _context.Categorias
                .AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower());
        }
    }
}