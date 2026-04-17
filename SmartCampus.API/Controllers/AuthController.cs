using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCampus.API.Domain.Entities;
using SmartCampus.API.DTOs;
using SmartCampus.API.Persistence.Context;

namespace SmartCampus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SmartCampusDbContext _context;

        public AuthController(SmartCampusDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (_context.Usuarios.Any(u => u.Email == dto.Email))
            {
                return BadRequest(new { mensaje = "El correo ya existe" });
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                IdRol = dto.IdRol
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            // Si es estudiante (ejemplo: rol 3)
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
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            }

            var response = new LoginResponseDto
            {
                IdUsuario = user.IdUsuario,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol.NombreRol,
                Token = "fake-token"
            };

            return Ok(response);
        }
    }
}