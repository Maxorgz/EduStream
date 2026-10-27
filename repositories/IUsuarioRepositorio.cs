using EduStream.Models;

namespace EduStream.Repositories
{
    public interface IUsuarioRepositorio
    {
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<Usuario?> ObtenerPorCredencialesAsync(string email, string password);
        Task<IEnumerable<Usuario>> BuscarPorRolYNombreAsync(string rol, string termino);
        Task<bool> ExisteEmailAsync(string email);
        Task AgregarAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
    }
}