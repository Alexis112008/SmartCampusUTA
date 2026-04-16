using SmartCampus.API.Domain.DataStructures;
using SmartCampus.API.DTOs;
using SmartCampus.API.Persistence.Repositories;

namespace SmartCampus.API.Application.Services
{
    public class CampusService
    {
        private readonly CampusRepository _repository;

        public CampusService(CampusRepository repository)
        {
            _repository = repository;
        }

        // Construir el Grafo desde la base de datos
        private async Task<Grafo> ConstruirGrafoAsync()
        {
            var grafo = new Grafo();

            var nodos = await _repository.ObtenerNodosAsync();
            foreach (var n in nodos)
                grafo.AgregarNodo(n.IdNodo, n.Nombre, n.PosX, n.PosY);

            var rutas = await _repository.ObtenerRutasAsync();
            foreach (var r in rutas)
                grafo.AgregarArista(
                    r.IdNodoOrigen, r.IdNodoDestino, r.Distancia);

            return grafo;
        }

        // Buscar ruta entre dos puntos del campus
        public async Task<RutaResponseDto> BuscarRutaAsync(
            RutaRequestDto dto)
        {
            var grafo = await ConstruirGrafoAsync();

            var pasos = grafo.BuscarRuta(
                dto.IdNodoOrigen, dto.IdNodoDestino);

            return new RutaResponseDto
            {
                Pasos = pasos,
                Mensaje = pasos.Count > 1
                    ? $"Ruta encontrada: {pasos.Count - 1} tramo(s)"
                    : pasos[0]
            };
        }

        // Obtener todos los nodos para dibujar el mapa
        public async Task<List<NodoResponseDto>> ObtenerNodosAsync()
        {
            var nodos = await _repository.ObtenerNodosAsync();
            return nodos.Select(n => new NodoResponseDto
            {
                IdNodo = n.IdNodo,
                Nombre = n.Nombre,
                Descripcion = n.Descripcion,
                PosX = n.PosX,
                PosY = n.PosY
            }).ToList();
        }
    }
}