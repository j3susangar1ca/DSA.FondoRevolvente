using HCG.FondoRevolvente.Application.DTOs;
using HCG.FondoRevolvente.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HCG.FondoRevolvente.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitudesController : ControllerBase
{
    private readonly SolicitudService _solicitudService;

    public SolicitudesController(SolicitudService solicitudService)
    {
        _solicitudService = solicitudService;
    }

    [HttpPost]
    public async Task<IActionResult> Crear()
    {
        var usuarioSimuladoId = "USR-COMPRADOR-01";
        var solicitud = await _solicitudService.CrearNuevaSolicitudAsync(usuarioSimuladoId);
        return CreatedAtAction(nameof(Obtener), new { id = solicitud.Id }, solicitud);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        try
        {
            var solicitud = await _solicitudService.ObtenerSolicitudAsync(id);
            return Ok(solicitud);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] Solicitud solicitudActualizada)
    {
        try
        {
            var usuarioSimuladoId = "USR-COMPRADOR-01";
            await _solicitudService.ActualizarSolicitudAsync(id, solicitudActualizada, usuarioSimuladoId);
            return Ok(new { Mensaje = "Expediente actualizado exitosamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{id}/desbloquear")]
    public async Task<IActionResult> Desbloquear(int id)
    {
        try
        {
            var usuarioSimuladoId = "USR-COMPRADOR-01";
            await _solicitudService.DesbloquearSolicitudAsync(id, usuarioSimuladoId);
            return Ok(new { Mensaje = "Bloqueo liberado exitosamente." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
