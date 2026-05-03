namespace SmartCampus.API.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}

public class SolicitarRecuperacionDto
{
    public string Email { get; set; }
}

public class VerificarTokenDto
{
    public string Token { get; set; }
}

public class CambiarPasswordDto
{
    public string Token { get; set; }
    public string NuevaPassword { get; set; }
}