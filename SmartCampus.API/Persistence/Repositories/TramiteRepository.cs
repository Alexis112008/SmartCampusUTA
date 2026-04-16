using Microsoft.EntityFrameworkCore;
using SmartCampus.API.Domain.Entities;
using SmartCampus.API.Persistence.Context;

namespace SmartCampus.API.Persistence.Repositories
{
    public class TramiteRepository
    {
        private readonly SmartCampusDbContext _context;

        public TramiteRepository(SmartCampusDbContext context)
        {
            _context = context;
        }

        // Obtener todos los trámites con sus relaciones
        public async Task<List<Tramite>> ObtenerTodosAsync()
        {
            return await _context.Tramites
                .Include(t => t.Estudiante)
                    .ThenInclude(e => e.Usuario)
                .Include(t => t.Historial)
                    .ThenInclude(h => h.Usuario)
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();
        }

        // Obtener trámites de un estudiante específico
        public async Task<List<Tramite>> ObtenerPorEstudianteAsync(
            int idEstudiante)
        {
            return await _context.Tramites
                .Include(t => t.Historial)
                    .ThenInclude(h => h.Usuario)
                .Where(t => t.IdEstudiante == idEstudiante)
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();
        }

        // Obtener un trámite por ID
        public async Task<Tramite?> ObtenerPorIdAsync(int id)
        {
            return await _context.Tramites
                .Include(t => t.Estudiante)
                    .ThenInclude(e => e.Usuario)
                .Include(t => t.Historial.OrderBy(h => h.FechaCambio))
                    .ThenInclude(h => h.Usuario)
                .FirstOrDefaultAsync(t => t.IdTramite == id);
        }

        // Crear un nuevo trámite
        public async Task<Tramite> CrearAsync(Tramite tramite)
        {
            _context.Tramites.Add(tramite);
            await _context.SaveChangesAsync();
            return tramite;
        }

        // Agregar un estado al historial
        public async Task AgregarHistorialAsync(
            HistorialTramite historial)
        {
            _context.HistorialTramites.Add(historial);
            // También actualizar el estado actual del trámite
            var tramite = await _context.Tramites
                .FindAsync(historial.IdTramite);
            if (tramite != null)
            {
                tramite.Estado = historial.Estado;
                _context.Tramites.Update(tramite);
            }
            await _context.SaveChangesAsync();
        }
    }
}