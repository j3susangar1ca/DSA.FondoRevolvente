using HCG.FondoRevolvente.Application.DTOs;
using HCG.FondoRevolvente.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HCG.FondoRevolvente.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitudesController : ControllerBase
{
    private readonly ISolicitudService _solicitudService;

    public SolicitudesController(ISolicitudService solicitudService)
    {
        _solicitudService = solicitudService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SolicitudDto>>> GetAll()
    {
        var solicitudes = await _solicitudService.GetAllAsync();
        return Ok(solicitudes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SolicitudDto>> GetById(int id)
    {
        var solicitud = await _solicitudService.GetByIdAsync(id);
        if (solicitud == null) return NotFound();
        return Ok(solicitud);
    }

    [HttpPost]
    public async Task<ActionResult<SolicitudDto>> Create(CreateSolicitudDto dto)
    {
        // For now, using a placeholder user identity. In a real scenario, this would come from JWT claims.
        var usuarioActualId = "user-123"; 
        var solicitud = await _solicitudService.CreateAsync(dto, usuarioActualId);
        return CreatedAtAction(nameof(GetById), new { id = solicitud.Id }, solicitud);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id)
    {
        var usuarioActualId = "user-123";
        try
        {
            await _solicitudService.UpdateAsync(id, usuarioActualId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
