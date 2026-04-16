using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class NodoCampus
    {
        [Key]
        public int IdNodo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }

        public ICollection<RutaCampus> RutasOrigen { get; set; }
            = new List<RutaCampus>();
        public ICollection<RutaCampus> RutasDestino { get; set; }
            = new List<RutaCampus>();
    }
}