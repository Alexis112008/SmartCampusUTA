using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class AccionUsuario
    {
        [Key]
        public int IdAccion { get; set; }
        public int IdUsuario { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public string? DatoAnterior { get; set; }
        public DateTime FechaAccion { get; set; } = DateTime.Now;
        public bool Revertida { get; set; } = false;

        public Usuario Usuario { get; set; } = null!;
    }
}