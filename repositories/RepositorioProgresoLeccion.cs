using EduStream.Data;
using EduStream.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStream.Repositories
{
    public class RepositorioProgresoLeccion : RepositorioBase, IRepositorioProgresoLeccion
    {
        public RepositorioProgresoLeccion(EduStreamDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProgresoLeccion>> ObtenerPorInscripcionIdAsync(int inscripcionId)
        {
            return await _context.ProgresosLecciones
                .Include(p => p.Leccion)
                .Where(p => p.InscripcionId == inscripcionId)
                .ToListAsync();
        }

        public async Task<ProgresoLeccion?> ObtenerPorInscripcionYLeccionAsync(int inscripcionId, int leccionId)
        {
            return await _context.ProgresosLecciones
                .Include(p => p.Leccion)
                .FirstOrDefaultAsync(p => p.InscripcionId == inscripcionId && p.LeccionId == leccionId);
        }

        public async Task<bool> GuardarOActualizarProgresoAsync(int inscripcionId, int leccionId, bool completada)
        {
            var progreso = await _context.ProgresosLecciones
                .FirstOrDefaultAsync(p => p.InscripcionId == inscripcionId && p.LeccionId == leccionId);

            if (progreso == null)
            {
                progreso = new ProgresoLeccion
                {
                    InscripcionId = inscripcionId,
                    LeccionId = leccionId,
                    Completada = completada,
                    FechaUltimoAcceso = DateTime.Now
                };
                _context.ProgresosLecciones.Add(progreso);
            }
            else
            {
                progreso.Completada = completada;
                progreso.FechaUltimoAcceso = DateTime.Now;
                _context.ProgresosLecciones.Update(progreso);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> ObtenerTotalLeccionesCompletadasAsync(int inscripcionId)
        {
            return await _context.ProgresosLecciones
                .Where(p => p.InscripcionId == inscripcionId && p.Completada)
                .CountAsync();
        }
    }
}