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
        var solicitud = await _solicitudService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = solicitud.Id }, solicitud);
    }
}
