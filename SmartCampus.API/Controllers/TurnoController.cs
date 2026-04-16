using Microsoft.AspNetCore.Mvc;
using SmartCampus.API.Application.Services;
using SmartCampus.API.DTOs;

namespace SmartCampus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurnoController : ControllerBase
    {
        private readonly TurnoService _service;

        public TurnoController(TurnoService service)
        {
            _service = service;
        }

        // POST api/turno/solicitar
        [HttpPost("solicitar")]
        public async Task<IActionResult> Solicitar(
            [FromBody] SolicitarTurnoDto dto)
        {
            var turno = await _service.SolicitarTurnoAsync(dto);
            return Ok(turno);
        }

        // POST api/turno/atender/{idVentanilla}
        [HttpPost("atender/{idVentanilla}")]
        public async Task<IActionResult> Atender(int idVentanilla)
        {
            var turno = await _service.AtenderSiguienteAsync(idVentanilla);
            if (turno == null)
                return NotFound(new { mensaje = "No hay turnos en espera" });

            return Ok(turno);
        }

        // GET api/turno/cola/{idVentanilla}
        [HttpGet("cola/{idVentanilla}")]
        public async Task<IActionResult> VerCola(int idVentanilla)
        {
            var cola = await _service.VerColaAsync(idVentanilla);
            return Ok(cola);
        }
    }
}