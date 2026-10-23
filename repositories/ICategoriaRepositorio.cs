using EduStream.Models;

namespace EduStream.Repositories
{
    public interface ICategoriaRepositorio
    {
        Task<IEnumerable<Categoria>> ObtenerTodasAsync();
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task<Categoria?> ObtenerConCursosAsync(int id); // Útil para ver los cursos que pertenecen a una categoría
        Task<Categoria> CrearAsync(Categoria categoria);
        Task<bool> ActualizarAsync(Categoria categoria);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteNombreAsync(string nombre);
    }
}