using SmartCampus.API.Domain.DataStructures;
using SmartCampus.API.Domain.Entities;
using SmartCampus.API.DTOs;
using SmartCampus.API.Persistence.Repositories;

namespace SmartCampus.API.Application.Services
{
    public class TurnoService
    {
        private readonly TurnoRepository _repository;

        // Lista Circular en memoria para rotar ventanillas
        private static ListaCircular<int> _rotacionVentanillas
            = new ListaCircular<int>();
        private static bool _ventanillasInicializadas = false;

        public TurnoService(TurnoRepository repository)
        {
            _repository = repository;
        }

        // Solicitar turno usando Cola FIFO
        public async Task<TurnoResponseDto> SolicitarTurnoAsync(
            SolicitarTurnoDto dto)
        {
            // Inicializar rotación si no está lista
            await InicializarRotacionAsync();

            // Obtener siguiente ventanilla en rotación (Lista Circular)
            int idVentanillaAsignada = dto.IdVentanilla;
            if (dto.IdVentanilla == 0)
            {
                // Si no especifica ventanilla, asignar automáticamente
                idVentanillaAsignada = _rotacionVentanillas
                    .ObtenerSiguienteEnRotacion();
            }

            // Obtener turnos en espera y cargarlos en la Cola
            var turnosEnEspera = await _repository
                .ObtenerEnEsperaAsync(idVentanillaAsignada);

            var cola = new Cola<Turno>();
            foreach (var t in turnosEnEspera)
                cola.Encolar(t);

            // Calcular el siguiente número de turno
            var ultimoNumero = await _repository
                .ObtenerUltimoNumeroAsync(idVentanillaAsignada);

            var nuevoTurno = new Turno
            {
                IdEstudiante = dto.IdEstudiante,
                IdVentanilla = idVentanillaAsignada,
                NumeroTurno = ultimoNumero + 1,
                Estado = "Espera"
            };

            var creado = await _repository.CrearAsync(nuevoTurno);

            // Recargar con relaciones para la respuesta
            var turnosActualizados = await _repository
                .ObtenerEnEsperaAsync(idVentanillaAsignada);

            var ventanilla = turnosActualizados
                .FirstOrDefault()?.Ventanilla;

            return new TurnoResponseDto
            {
                IdTurno = creado.IdTurno,
                NumeroTurno = creado.NumeroTurno,
                Estado = creado.Estado,
                NombreVentanilla = ventanilla?.Nombre ?? "Ventanilla",
                FechaSolicitud = creado.FechaSolicitud
            };
        }

        // Atender siguiente turno (desencolar)
        public async Task<TurnoResponseDto?> AtenderSiguienteAsync(
            int idVentanilla)
        {
            // Cargar cola desde BD
            var turnosEnEspera = await _repository
                .ObtenerEnEsperaAsync(idVentanilla);

            if (turnosEnEspera.Count == 0)
                return null;

            // Construir la Cola con los turnos
            var cola = new Cola<Turno>();
            foreach (var t in turnosEnEspera)
                cola.Encolar(t);

            // Desencolar el primero (FIFO)
            var siguiente = cola.Desencolar();

            // Actualizar en base de datos
            await _repository.AtenderSiguienteAsync(idVentanilla);

            return new TurnoResponseDto
            {
                IdTurno = siguiente.IdTurno,
                NumeroTurno = siguiente.NumeroTurno,
                Estado = "Atendido",
                NombreEstudiante = siguiente.Estudiante?.Usuario?.Nombre
                                   ?? "Estudiante",
                NombreVentanilla = siguiente.Ventanilla?.Nombre
                                   ?? "Ventanilla",
                FechaSolicitud = siguiente.FechaSolicitud
            };
        }

        // Ver cola actual de una ventanilla
        public async Task<List<TurnoResponseDto>> VerColaAsync(
            int idVentanilla)
        {
            var turnos = await _repository
                .ObtenerEnEsperaAsync(idVentanilla);

            var cola = new Cola<Turno>();
            foreach (var t in turnos)
                cola.Encolar(t);

            return cola.ObtenerTodos().Select(t => new TurnoResponseDto
            {
                IdTurno = t.IdTurno,
                NumeroTurno = t.NumeroTurno,
                Estado = t.Estado,
                NombreEstudiante = t.Estudiante?.Usuario?.Nombre
                                   ?? "Estudiante",
                NombreVentanilla = t.Ventanilla?.Nombre ?? "Ventanilla",
                FechaSolicitud = t.FechaSolicitud
            }).ToList();
        }

        // Inicializar Lista Circular con las ventanillas activas
        private async Task InicializarRotacionAsync()
        {
            if (_ventanillasInicializadas) return;

            var ventanillas = await _repository
                .ObtenerVentanillasActivasAsync();

            foreach (var v in ventanillas)
                _rotacionVentanillas.Insertar(v.IdVentanilla);

            _ventanillasInicializadas = true;
        }
    }
}