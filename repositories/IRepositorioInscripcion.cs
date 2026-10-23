using EduStream.Models;

namespace EduStream.Repositories
{
    public interface IRepositorioInscripcion
    {
        Task<IEnumerable<Inscripcion>> ObtenerTodasAsync();
        Task<Inscripcion?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Inscripcion>> ObtenerPorEstudianteIdAsync(int estudianteId);
        Task<IEnumerable<Inscripcion>> ObtenerPorCursoIdAsync(int cursoId);
        Task<Inscripcion> CrearAsync(Inscripcion inscripcion);
        Task<bool> ActualizarAsync(Inscripcion inscripcion);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteInscripcionAsync(int estudianteId, int cursoId);
    }
}