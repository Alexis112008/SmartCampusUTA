using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class PersonalAdministrativo
    {
        [Key]
        public int IdPersonal { get; set; }
        public int IdUsuario { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;

        public Usuario Usuario { get; set; } = null!;
    }
}