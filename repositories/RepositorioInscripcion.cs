using EduStream.Data;
using EduStream.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStream.Repositories
{
    public class RepositorioInscripcion : RepositorioBase, IRepositorioInscripcion
    {
        public RepositorioInscripcion(EduStreamDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Inscripcion>> ObtenerTodasAsync()
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Curso)
                .ToListAsync();
        }

        public async Task<Inscripcion?> ObtenerPorIdAsync(int id)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Curso)
                .Include(i => i.Progresos)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Inscripcion>> ObtenerPorEstudianteIdAsync(int estudianteId)
        {
            return await _context.Inscripciones
                .Include(i => i.Curso)
                .Where(i => i.EstudianteId == estudianteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inscripcion>> ObtenerPorCursoIdAsync(int cursoId)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Where(i => i.CursoId == cursoId)
                .ToListAsync();
        }

        public async Task<Inscripcion> CrearAsync(Inscripcion inscripcion)
        {
            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();
            return inscripcion;
        }

        public async Task<bool> ActualizarAsync(Inscripcion inscripcion)
        {
            _context.Inscripciones.Update(inscripcion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion == null) return false;

            _context.Inscripciones.Remove(inscripcion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExisteInscripcionAsync(int estudianteId, int cursoId)
        {
            return await _context.Inscripciones
                .AnyAsync(i => i.EstudianteId == estudianteId && i.CursoId == cursoId);
        }
    }
}