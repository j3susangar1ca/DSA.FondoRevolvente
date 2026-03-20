using HCG.FondoRevolvente.Application.Interfaces;
using HCG.FondoRevolvente.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace HCG.FondoRevolvente.Application.Services;

public class SolicitudService
{
    private readonly ISolicitudRepository _repository;

    public SolicitudService(ISolicitudRepository repository)
    {
        _repository = repository;
    }

    public async Task<Solicitud> CrearNuevaSolicitudAsync(string usuarioActualId)
    {
        var nuevaSolicitud = new Solicitud();
        nuevaSolicitud.BloquearParaEdicion(usuarioActualId);
        await _repository.AgregarAsync(nuevaSolicitud);
        return nuevaSolicitud;
    }

    public async Task<Solicitud> ObtenerSolicitudAsync(int id)
    {
        var solicitud = await _repository.ObtenerPorIdAsync(id);
        if (solicitud == null)
        {
            throw new KeyNotFoundException($"Solicitud con ID {id} no encontrada.");
        }
        return solicitud;
    }

    public async Task ActualizarSolicitudAsync(int id, Solicitud solicitudActualizada, string usuarioActualId)
    {
        var solicitudDb = await _repository.ObtenerPorIdAsync(id);
        if (solicitudDb == null) throw new KeyNotFoundException("La solicitud no existe.");

        if (!solicitudDb.PuedeSerEditadaPor(usuarioActualId))
        {
            throw new InvalidOperationException($"El expediente está bloqueado y siendo editado por {solicitudDb.BloqueadoPor}");
        }

        // Copiar propiedades relevantes
        solicitudDb.Partidas.Clear();
        foreach (var partida in solicitudActualizada.Partidas)
        {
            solicitudDb.AgregarPartida(partida);
        }

        if (solicitudDb.ExcedeLimitePermitido())
        {
            throw new InvalidOperationException("El monto total excede el límite permitido de $75,000.00 MXN para operaciones de Fondo Revolvente.");
        }

        await _repository.ActualizarAsync(solicitudDb);
    }

    public async Task DesbloquearSolicitudAsync(int id, string usuarioActualId)
    {
        var solicitud = await _repository.ObtenerPorIdAsync(id);
        if (solicitud == null) throw new KeyNotFoundException("La solicitud no existe.");

        if (solicitud.BloqueadoPor == usuarioActualId)
        {
            solicitud.LiberarBloqueo();
            await _repository.ActualizarAsync(solicitud);
        }
    }
}
