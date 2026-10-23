using EduStream.Data;
using EduStream.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStream.Repositories
{
    public class RepositorioMaterialEstudio : RepositorioBase, IMaterialEstudioRepositorio
    {
        public RepositorioMaterialEstudio(EduStreamDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MaterialEstudio>> ObtenerPorLeccionIdAsync(int leccionId)
        {
            return await _context.MaterialesEstudio
                .Where(m => m.LeccionId == leccionId)
                .ToListAsync();
        }

        public async Task<MaterialEstudio?> ObtenerPorIdAsync(int id)
        {
            return await _context.MaterialesEstudio
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MaterialEstudio> CrearAsync(MaterialEstudio material)
        {
            _context.MaterialesEstudio.Add(material);
            await _context.SaveChangesAsync();
            return material;
        }

        public async Task<bool> ActualizarAsync(MaterialEstudio material)
        {
            _context.MaterialesEstudio.Update(material);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var material = await _context.MaterialesEstudio.FindAsync(id);
            if (material == null) return false;

            _context.MaterialesEstudio.Remove(material);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}