using EduStream.Models;

namespace EduStream.Repositories
{
    public interface IRepositorioProgresoLeccion
    {
        Task<IEnumerable<ProgresoLeccion>> ObtenerPorInscripcionIdAsync(int inscripcionId);
        Task<ProgresoLeccion?> ObtenerPorInscripcionYLeccionAsync(int inscripcionId, int leccionId);
        Task<bool> RegistrarOActualizarProgresoAsync(ProgresoLeccion progreso);
    }
}