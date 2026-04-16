using Microsoft.EntityFrameworkCore;
using SmartCampus.API.Domain.Entities;
using SmartCampus.API.Persistence.Context;

namespace SmartCampus.API.Persistence.Repositories
{
    public class CampusRepository
    {
        private readonly SmartCampusDbContext _context;

        public CampusRepository(SmartCampusDbContext context)
        {
            _context = context;
        }

        public async Task<List<NodoCampus>> ObtenerNodosAsync()
        {
            return await _context.NodosCampus.ToListAsync();
        }

        public async Task<List<RutaCampus>> ObtenerRutasAsync()
        {
            return await _context.RutasCampus
                .Include(r => r.NodoOrigen)
                .Include(r => r.NodoDestino)
                .ToListAsync();
        }
    }
}