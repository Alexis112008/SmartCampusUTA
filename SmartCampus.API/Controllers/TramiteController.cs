using Microsoft.AspNetCore.Mvc;
using SmartCampus.API.Application.Services;
using SmartCampus.API.DTOs;

namespace SmartCampus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TramiteController : ControllerBase
    {
        private readonly TramiteService _service;

        public TramiteController(TramiteService service)
        {
            _service = service;
        }

        // GET api/tramite
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var tramites = await _service.ObtenerTodosAsync();
            return Ok(tramites);
        }

        // GET api/tramite/5/historial
        // Devuelve el trámite con su historial en Lista Simple
        [HttpGet("{id}/historial")]
        public async Task<IActionResult> ObtenerConHistorial(int id)
        {
            var tramite = await _service.ObtenerConHistorialAsync(id);
            if (tramite == null)
                return NotFound(new { mensaje = "Trámite no encontrado" });

            return Ok(tramite);
        }

        // POST api/tramite
        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] CrearTramiteDto dto)
        {
            // IdUsuario = 1 temporalmente (luego vendrá del token JWT)
            var tramite = await _service.CrearTramiteAsync(dto, 1);
            return CreatedAtAction(
                nameof(ObtenerConHistorial),
                new { id = tramite.IdTramite },
                tramite);
        }

        // PUT api/tramite/5/estado
        [HttpPut("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(
            int id, [FromBody] CambiarEstadoDto dto)
        {
            var resultado = await _service.CambiarEstadoAsync(id, dto);
            if (!resultado)
                return NotFound(new { mensaje = "Trámite no encontrado" });

            return Ok(new { mensaje = "Estado actualizado correctamente" });
        }
    }
}