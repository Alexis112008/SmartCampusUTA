using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class Tramite
    {
        [Key]
        public int IdTramite { get; set; }
        public int IdEstudiante { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = "Pendiente";
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public Estudiante Estudiante { get; set; } = null!;
        public ICollection<HistorialTramite> Historial { get; set; }
            = new List<HistorialTramite>();
    }
}