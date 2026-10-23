using EduStream.Models;

namespace EduStream.Repositories
{
    public interface ILeccionRepositorio
    {
        Task<IEnumerable<Leccion>> ObtenerPorCursoIdAsync(int cursoId);
        Task<Leccion?> ObtenerPorIdAsync(int id);
        Task<Leccion?> ObtenerLeccionConMaterialesAsync(int id); // Incluye los materiales de estudio asociados
        Task<Leccion> CrearAsync(Leccion leccion);
        Task<bool> ActualizarAsync(Leccion leccion);
        Task<bool> EliminarAsync(int id);
    }
}