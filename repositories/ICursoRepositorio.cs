using EduStream.Models;

namespace EduStream.Repositories
{
    public interface ICursoRepositorio
    {
        Task<IEnumerable<Curso>> ObtenerTodosAsync();
        Task<Curso?> ObtenerPorIdAsync(int id);
        Task<Curso?> ObtenerCursoConDetallesAsync(int id); // Incluye lecciones y materiales
        Task<IEnumerable<Curso>> ObtenerPorCategoriaIdAsync(int categoriaId);
        Task<IEnumerable<Curso>> ObtenerPorProfesorIdAsync(int profesorId);
        Task<Curso> CrearAsync(Curso curso);
        Task<bool> ActualizarAsync(Curso curso);
        Task<bool> EliminarAsync(int id);
        Task<IEnumerable<Curso>> BuscarAsync(string searchTerm);
    }
}