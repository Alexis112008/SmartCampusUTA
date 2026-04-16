using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class Turno
    {
        [Key]
        public int IdTurno { get; set; }
        public int IdEstudiante { get; set; }
        public int IdVentanilla { get; set; }
        public int NumeroTurno { get; set; }
        public string Estado { get; set; } = "Espera";
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public DateTime? FechaAtencion { get; set; }

        public Estudiante Estudiante { get; set; } = null!;
        public Ventanilla Ventanilla { get; set; } = null!;
    }
}