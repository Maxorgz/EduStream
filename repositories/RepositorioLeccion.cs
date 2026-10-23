using EduStream.Data;
using EduStream.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStream.Repositories
{
    public class RepositorioLeccion : RepositorioBase, ILeccionRepositorio
    {
        public RepositorioLeccion(EduStreamDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Leccion>> ObtenerPorCursoIdAsync(int cursoId)
        {
            return await _context.Lecciones
                .Where(l => l.CursoId == cursoId)
                .OrderBy(l => l.Orden)
                .ToListAsync();
        }

        public async Task<Leccion?> ObtenerPorIdAsync(int id)
        {
            return await _context.Lecciones
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Leccion?> ObtenerLeccionConMaterialesAsync(int id)
        {
            return await _context.Lecciones
                .Include(l => l.Materiales)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Leccion> CrearAsync(Leccion leccion)
        {
            _context.Lecciones.Add(leccion);
            await _context.SaveChangesAsync();
            return leccion;
        }

        public async Task<bool> ActualizarAsync(Leccion leccion)
        {
            _context.Lecciones.Update(leccion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var leccion = await _context.Lecciones.FindAsync(id);
            if (leccion == null) return false;

            _context.Lecciones.Remove(leccion);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}