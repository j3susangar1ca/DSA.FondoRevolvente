using HCG.FondoRevolvente.Application.DTOs;
using HCG.FondoRevolvente.Application.Interfaces;
using HCG.FondoRevolvente.Domain.Constants;
using HCG.FondoRevolvente.Domain.Entities;
using HCG.FondoRevolvente.Domain.Enums;

namespace HCG.FondoRevolvente.Application.Services;

public interface ISolicitudService
{
    Task<SolicitudDto> CreateAsync(CreateSolicitudDto dto, string usuarioActualId);
    Task<SolicitudDto?> GetByIdAsync(int id);
    Task<IEnumerable<SolicitudDto>> GetAllAsync();
    Task UpdateAsync(int id, string usuarioActualId);
}

public class SolicitudService : ISolicitudService
{
    private readonly ISolicitudRepository _repository;

    public SolicitudService(ISolicitudRepository repository)
    {
        _repository = repository;
    }

    public async Task<SolicitudDto> CreateAsync(CreateSolicitudDto dto, string usuarioActualId)
    {
        var solicitud = new Solicitud
        {
            ConceptoGeneral = dto.ConceptoGeneral,
            Justificacion = dto.Justificacion,
            AreaSolicitante = dto.AreaSolicitante,
            NombreResponsable = dto.NombreResponsable,
            FechaRequerida = dto.FechaRequerida,
            Folio = await GenerateFolioAsync()
        };

        foreach (var p in dto.Partidas)
        {
            solicitud.Partidas.Add(new Partida
            {
                CodigoProducto = p.CodigoProducto,
                Descripcion = p.Descripcion,
                Cantidad = (int)p.Cantidad,
                PrecioUnitario = p.PrecioUnitario
            });
        }

        // Pragmatic logic from user:
        solicitud.BloquearParaEdicion(usuarioActualId);
        await AplicarReglasNegocio(solicitud, usuarioActualId);
        
        await _repository.AddAsync(solicitud);

        return MapToDto(solicitud);
    }

    public async Task UpdateAsync(int id, string usuarioActualId)
    {
        var solicitud = await _repository.GetByIdAsync(id);
        if (solicitud == null) throw new Exception("Solicitud no encontrada.");

        if (!solicitud.PuedeSerEditadaPor(usuarioActualId))
        {
            throw new Exception($"El expediente está siendo editado por {solicitud.BloqueadoPor ?? "desconocido"}.");
        }

        await AplicarReglasNegocio(solicitud, usuarioActualId);
        solicitud.BloquearParaEdicion(usuarioActualId); // Renovar bloqueo
        await _repository.SaveAsync(solicitud);
    }

    private async Task AplicarReglasNegocio(Solicitud solicitud, string usuarioActualId)
    {
        // RN-001: Validación de límite financiero
        if (solicitud.ExcedeLimitePermitido())
        {
            throw new Exception($"El monto total ({solicitud.TotalMonto:C}) excede el límite permitido de {LimitesNegocio.MONTO_MAXIMO_SOLICITUD:C}.");
        }

        // RN-005: Verificación de permiso de edición
        if (!solicitud.PuedeSerEditadaPor(usuarioActualId))
        {
             throw new Exception("La solicitud está bloqueada para edición por otro usuario.");
        }
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
