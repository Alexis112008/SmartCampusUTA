using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class HistorialTramite
    {
        [Key]
        public int IdHistorial { get; set; }
        public int IdTramite { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observacion { get; set; }
        public DateTime FechaCambio { get; set; } = DateTime.Now;
        public int IdUsuario { get; set; }

        public Tramite Tramite { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}