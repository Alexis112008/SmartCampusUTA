using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public Rol Rol { get; set; } = null!;
        public Estudiante? Estudiante { get; set; }
        public PersonalAdministrativo? Personal { get; set; }
    }
}