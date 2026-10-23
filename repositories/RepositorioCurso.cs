using EduStream.Data;
using EduStream.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStream.Repositories
{
    public class RepositorioCurso : RepositorioBase, ICursoRepositorio
    {
        public RepositorioCurso(EduStreamDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Curso>> ObtenerTodosAsync()
        {
            return await _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Profesor)
                .ToListAsync();
        }

        public async Task<Curso?> ObtenerPorIdAsync(int id)
        {
            return await _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Profesor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Curso?> ObtenerCursoConDetallesAsync(int id)
        {
    return await _context.Cursos
        .Include(c => c.Categoria)
        .Include(c => c.Profesor)
        .Include(c => (c.Lecciones ?? new List<Leccion>()).OrderBy(l => l.Orden))
            .ThenInclude(l => l.Materiales)
        .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Curso>> ObtenerPorCategoriaIdAsync(int categoriaId)
        {
            return await _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Profesor)
                .Where(c => c.CategoriaId == categoriaId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curso>> ObtenerPorProfesorIdAsync(int profesorId)
        {
            return await _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Profesor)
                .Where(c => c.ProfesorId == profesorId)
                .ToListAsync();
        }

        public async Task<Curso> CrearAsync(Curso curso)
        {
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();
            return curso;
        }

        public async Task<bool> ActualizarAsync(Curso curso)
        {
            _context.Cursos.Update(curso);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return false;

            _context.Cursos.Remove(curso);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Curso>> BuscarAsync(string searchTerm)
        {
            return await _context.Cursos
                .Include(c => c.Categoria)
                .Include(c => c.Profesor)
                .Where(c => c.Titulo.Contains(searchTerm) || c.Descripcion.Contains(searchTerm))
                .ToListAsync();
        }
    }
}