namespace SmartCampus.API.DTOs
{
    // DTO para crear un trámite nuevo
    public class CrearTramiteDto
    {
        public int IdEstudiante { get; set; }
        public string Tipo { get; set; } = string.Empty;
    }

    // DTO para cambiar el estado de un trámite
    public class CambiarEstadoDto
    {
        public string Estado { get; set; } = string.Empty;
        public string? Observacion { get; set; }
        public int IdUsuario { get; set; }
    }

    // DTO de respuesta: lo que devuelve la API
    public class TramiteResponseDto
    {
        public int IdTramite { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public List<HistorialItemDto> Historial { get; set; } = new();
    }

    public class HistorialItemDto
    {
        public string Estado { get; set; } = string.Empty;
        public string? Observacion { get; set; }
        public DateTime FechaCambio { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
    }
}