using Microsoft.AspNetCore.Mvc;
using SmartCampus.API.Application.Services;
using SmartCampus.API.DTOs;

namespace SmartCampus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampusController : ControllerBase
    {
        private readonly CampusService _service;

        public CampusController(CampusService service)
        {
            _service = service;
        }

        // GET api/campus/nodos
        [HttpGet("nodos")]
        public async Task<IActionResult> ObtenerNodos()
        {
            var nodos = await _service.ObtenerNodosAsync();
            return Ok(nodos);
        }

        // POST api/campus/ruta
        [HttpPost("ruta")]
        public async Task<IActionResult> BuscarRuta(
            [FromBody] RutaRequestDto dto)
        {
            var ruta = await _service.BuscarRutaAsync(dto);
            return Ok(ruta);
        }
    }
}