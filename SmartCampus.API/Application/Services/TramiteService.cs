using SmartCampus.API.Domain.DataStructures;
using SmartCampus.API.Domain.Entities;
using SmartCampus.API.DTOs;
using SmartCampus.API.Persistence.Repositories;

namespace SmartCampus.API.Application.Services
{
    public class TramiteService
    {
        private readonly TramiteRepository _repository;

        public TramiteService(TramiteRepository repository)
        {
            _repository = repository;
        }

        // Obtener historial usando Lista Simple
        // Aquí es donde la estructura de datos tiene uso REAL
        public async Task<TramiteResponseDto?> ObtenerConHistorialAsync(
            int idTramite)
        {
            var tramite = await _repository.ObtenerPorIdAsync(idTramite);
            if (tramite == null) return null;

            // Construimos la Lista Simple con el historial
            var listaHistorial = new ListaSimple<HistorialItemDto>();

            foreach (var item in tramite.Historial
                         .OrderBy(h => h.FechaCambio))
            {
                listaHistorial.InsertarAlFinal(new HistorialItemDto
                {
                    Estado = item.Estado,
                    Observacion = item.Observacion,
                    FechaCambio = item.FechaCambio,
                    NombreUsuario = item.Usuario?.Nombre ?? "Sistema"
                });
            }

            return new TramiteResponseDto
            {
                IdTramite = tramite.IdTramite,
                Tipo = tramite.Tipo,
                Estado = tramite.Estado,
                FechaCreacion = tramite.FechaCreacion,
                NombreEstudiante = tramite.Estudiante?.Usuario?.Nombre
                                   ?? "Desconocido",
                // ObtenerTodos() recorre la Lista Simple
                Historial = listaHistorial.ObtenerTodos()
            };
        }

        // Crear un nuevo trámite
        public async Task<Tramite> CrearTramiteAsync(
            CrearTramiteDto dto, int idUsuario)
        {
            var tramite = new Tramite
            {
                IdEstudiante = dto.IdEstudiante,
                Tipo = dto.Tipo,
                Estado = "Pendiente"
            };

            var creado = await _repository.CrearAsync(tramite);

            // Registrar el primer estado en el historial
            var historialInicial = new HistorialTramite
            {
                IdTramite = creado.IdTramite,
                Estado = "Pendiente",
                Observacion = "Trámite creado",
                IdUsuario = idUsuario
            };
            await _repository.AgregarHistorialAsync(historialInicial);

            return creado;
        }

        // Cambiar estado del trámite
        public async Task<bool> CambiarEstadoAsync(
            int idTramite, CambiarEstadoDto dto)
        {
            var tramite = await _repository.ObtenerPorIdAsync(idTramite);
            if (tramite == null) return false;

            var historial = new HistorialTramite
            {
                IdTramite = idTramite,
                Estado = dto.Estado,
                Observacion = dto.Observacion,
                IdUsuario = dto.IdUsuario
            };

            await _repository.AgregarHistorialAsync(historial);
            return true;
        }

        // Obtener todos los trámites
        public async Task<List<TramiteResponseDto>> ObtenerTodosAsync()
        {
            var tramites = await _repository.ObtenerTodosAsync();
            return tramites.Select(t => new TramiteResponseDto
            {
                IdTramite = t.IdTramite,
                Tipo = t.Tipo,
                Estado = t.Estado,
                FechaCreacion = t.FechaCreacion,
                NombreEstudiante = t.Estudiante?.Usuario?.Nombre
                                   ?? "Desconocido"
            }).ToList();
        }
    }
}