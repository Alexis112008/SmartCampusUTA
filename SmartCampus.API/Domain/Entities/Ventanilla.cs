using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class Ventanilla
    {
        [Key]
        public int IdVentanilla { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public bool Activa { get; set; } = true;

        public ICollection<Turno> Turnos { get; set; }
            = new List<Turno>();
    }
}