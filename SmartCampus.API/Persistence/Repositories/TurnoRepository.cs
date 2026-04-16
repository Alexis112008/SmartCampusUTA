using Microsoft.EntityFrameworkCore;
using SmartCampus.API.Domain.Entities;
using SmartCampus.API.Persistence.Context;

namespace SmartCampus.API.Persistence.Repositories
{
    public class TurnoRepository
    {
        private readonly SmartCampusDbContext _context;

        public TurnoRepository(SmartCampusDbContext context)
        {
            _context = context;
        }

        public async Task<List<Turno>> ObtenerEnEsperaAsync(
            int idVentanilla)
        {
            return await _context.Turnos
                .Include(t => t.Estudiante)
                    .ThenInclude(e => e.Usuario)
                .Include(t => t.Ventanilla)
                .Where(t => t.IdVentanilla == idVentanilla
                         && t.Estado == "Espera")
                .OrderBy(t => t.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<List<Ventanilla>> ObtenerVentanillasActivasAsync()
        {
            return await _context.Ventanillas
                .Where(v => v.Activa)
                .ToListAsync();
        }

        public async Task<int> ObtenerUltimoNumeroAsync(int idVentanilla)
        {
            var ultimo = await _context.Turnos
                .Where(t => t.IdVentanilla == idVentanilla)
                .OrderByDescending(t => t.NumeroTurno)
                .FirstOrDefaultAsync();
            return ultimo?.NumeroTurno ?? 0;
        }

        public async Task<Turno> CrearAsync(Turno turno)
        {
            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();
            return turno;
        }

        public async Task<bool> AtenderSiguienteAsync(int idVentanilla)
        {
            var turno = await _context.Turnos
                .Where(t => t.IdVentanilla == idVentanilla
                         && t.Estado == "Espera")
                .OrderBy(t => t.FechaSolicitud)
                .FirstOrDefaultAsync();

            if (turno == null) return false;

            turno.Estado = "Atendido";
            turno.FechaAtencion = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}