using EduStream.Models;

namespace EduStream.Repositories
{
    public interface IMaterialEstudioRepositorio
    {
        Task<IEnumerable<MaterialEstudio>> ObtenerPorLeccionIdAsync(int leccionId);
        Task<MaterialEstudio?> ObtenerPorIdAsync(int id);
        Task<MaterialEstudio> CrearAsync(MaterialEstudio material);
        Task<bool> ActualizarAsync(MaterialEstudio material);
        Task<bool> EliminarAsync(int id);
    }
}