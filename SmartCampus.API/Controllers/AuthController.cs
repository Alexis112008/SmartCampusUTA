using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCampus.API.Domain.Entities;
using SmartCampus.API.DTOs;
using SmartCampus.API.Persistence.Context;
using SmartCampus.API.Application.Services;

namespace SmartCampus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SmartCampusDbContext _context;
        private readonly RecuperacionService _recuperacionService;

        // ✅ UN solo constructor con todo
        public AuthController(
            SmartCampusDbContext context,
            RecuperacionService recuperacionService)
        {
            _context = context;
            _recuperacionService = recuperacionService;
        }

        [HttpPost("solicitar-recuperacion")]
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] SolicitarRecuperacionDto dto)
        {
            await _recuperacionService.SolicitarRecuperacion(dto.Email);
            return Ok(new { mensaje = "Si el correo existe, recibirás un código." });
        }

        [HttpPost("verificar-token")]
        public async Task<IActionResult> VerificarToken([FromBody] VerificarTokenDto dto)
        {
            var valido = await _recuperacionService.VerificarToken(dto.Token);
            if (!valido) return BadRequest(new { mensaje = "Código inválido o expirado." });
            return Ok(new { mensaje = "Código válido." });
        }

        [HttpPost("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto dto)
        {
            var resultado = await _recuperacionService.CambiarPassword(dto.Token, dto.NuevaPassword);
            if (!resultado) return BadRequest(new { mensaje = "Token inválido o expirado." });
            return Ok(new { mensaje = "Contraseña actualizada correctamente." });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (_context.Usuarios.Any(u => u.Email == dto.Email))
                return BadRequest(new { mensaje = "El correo ya existe" });

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                IdRol = dto.IdRol
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            if (dto.IdRol == 3)
            {
                var estudiante = new Estudiante
                {
                    IdUsuario = usuario.IdUsuario,
                    Cedula = dto.Cedula ?? "",
                    Carrera = dto.Carrera ?? "",
                    Semestre = dto.Semestre ?? 1,
                    FechaIngreso = DateOnly.FromDateTime(DateTime.Now)
                };
                _context.Estudiantes.Add(estudiante);
                _context.SaveChanges();
            }

            return Ok(new { mensaje = "Usuario registrado correctamente" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            var user = _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u =>
                    u.Email == login.Email &&
                    u.PasswordHash == login.PasswordHash);

            if (user == null)
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });

            return Ok(new LoginResponseDto
            {
                IdUsuario = user.IdUsuario,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol.NombreRol,
                Token = "fake-token"
            });
        }
    }
}