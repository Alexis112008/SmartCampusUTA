using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class Estudiante
    {
        [Key]
        public int IdEstudiante { get; set; }
        public int IdUsuario { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public int Semestre { get; set; }
        public DateOnly FechaIngreso { get; set; }

        public Usuario Usuario { get; set; } = null!;
        public ICollection<Tramite> Tramites { get; set; }
            = new List<Tramite>();
        public ICollection<Turno> Turnos { get; set; }
            = new List<Turno>();
    }
}