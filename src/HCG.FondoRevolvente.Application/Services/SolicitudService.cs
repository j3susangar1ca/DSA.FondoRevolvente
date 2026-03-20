using HCG.FondoRevolvente.Application.DTOs;
using HCG.FondoRevolvente.Application.Interfaces;
using HCG.FondoRevolvente.Domain.Entities;
using HCG.FondoRevolvente.Domain.Enums;

namespace HCG.FondoRevolvente.Application.Services;

public interface ISolicitudService
{
    Task<SolicitudDto> CreateAsync(CreateSolicitudDto dto);
    Task<SolicitudDto?> GetByIdAsync(int id);
    Task<IEnumerable<SolicitudDto>> GetAllAsync();
}

public class SolicitudService : ISolicitudService
{
    private readonly ISolicitudRepository _repository;

    public SolicitudService(ISolicitudRepository repository)
    {
        _repository = repository;
    }

    public async Task<SolicitudDto> CreateAsync(CreateSolicitudDto dto)
    {
        var solicitud = new Solicitud
        {
            ConceptoGeneral = dto.ConceptoGeneral,
            Justificacion = dto.Justificacion,
            AreaSolicitante = dto.AreaSolicitante,
            NombreResponsable = dto.NombreResponsable,
            FechaRequerida = dto.FechaRequerida,
            Estado = EstadoSolicitud.EN_REVISION_CAA, // As per README: initial status is pending CAA review unless specified as Borrador
            Folio = await GenerateFolioAsync(),
            Partidas = dto.Partidas.Select(p => new Partida
            {
                CodigoProducto = p.CodigoProducto,
                Descripcion = p.Descripcion,
                Cantidad = p.Cantidad,
                PrecioUnitario = p.PrecioUnitario
            }).ToList()
        };

        await _repository.AddAsync(solicitud);

        return MapToDto(solicitud);
    }

    public async Task<SolicitudDto?> GetByIdAsync(int id)
    {
        var solicitud = await _repository.GetByIdAsync(id);
        return solicitud != null ? MapToDto(solicitud) : null;
    }

    public async Task<IEnumerable<SolicitudDto>> GetAllAsync()
    {
        var solicitudes = await _repository.GetAllAsync();
        return solicitudes.Select(MapToDto);
    }

    private async Task<string> GenerateFolioAsync()
    {
        // Simple folio generation: DSA-YYYY-COUNT
        var solicitudes = await _repository.GetAllAsync();
        int count = solicitudes.Count() + 1;
        return $"DSA-{DateTime.Now.Year}-{count:D3}";
    }

    private static SolicitudDto MapToDto(Solicitud s) => new SolicitudDto
    {
        Id = s.Id,
        Folio = s.Folio,
        ConceptoGeneral = s.ConceptoGeneral,
        Justificacion = s.Justificacion,
        AreaSolicitante = s.AreaSolicitante,
        NombreResponsable = s.NombreResponsable,
        FechaCreacion = s.FechaCreacion,
        FechaRequerida = s.FechaRequerida,
        Estado = s.Estado,
        TotalMonto = s.TotalMonto
    };
}
