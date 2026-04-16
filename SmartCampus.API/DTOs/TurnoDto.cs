namespace SmartCampus.API.DTOs
{
    public class SolicitarTurnoDto
    {
        public int IdEstudiante { get; set; }
        public int IdVentanilla { get; set; }
    }

    public class TurnoResponseDto
    {
        public int IdTurno { get; set; }
        public int NumeroTurno { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string NombreVentanilla { get; set; } = string.Empty;
        public string NombreEstudiante { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
    }
}