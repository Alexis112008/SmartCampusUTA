namespace SmartCampus.API.DTOs
{
    public class RegisterDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int IdRol { get; set; }

        // Datos de estudiante
        public string? Cedula { get; set; }
        public string? Carrera { get; set; }
        public int? Semestre { get; set; }
    }
}