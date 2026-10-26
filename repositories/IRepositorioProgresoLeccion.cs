using EduStream.Models;

namespace EduStream.Repositories
{
    public interface IRepositorioProgresoLeccion
    {
        Task<IEnumerable<ProgresoLeccion>> ObtenerPorInscripcionIdAsync(int inscripcionId);
        Task<ProgresoLeccion?> ObtenerPorInscripcionYLeccionAsync(int inscripcionId, int leccionId);
        Task<bool> GuardarOActualizarProgresoAsync(int inscripcionId, int leccionId, bool completada);
        Task<int> ObtenerTotalLeccionesCompletadasAsync(int inscripcionId);
    }
}