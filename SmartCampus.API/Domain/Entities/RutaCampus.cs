using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class RutaCampus
    {
        [Key]
        public int IdRuta { get; set; }
        public int IdNodoOrigen { get; set; }
        public int IdNodoDestino { get; set; }
        public double Distancia { get; set; }
        public string? Descripcion { get; set; }

        public NodoCampus NodoOrigen { get; set; } = null!;
        public NodoCampus NodoDestino { get; set; } = null!;
    }
}