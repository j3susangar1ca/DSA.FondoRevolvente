using System.Collections.Generic;
using System.Linq;
using HCG.FondoRevolvente.Domain.Enums;
using HCG.FondoRevolvente.Domain.Constants;

namespace HCG.FondoRevolvente.Domain.Entities;

public class Solicitud
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public EstadoSolicitud Estado { get; private set; } = EstadoSolicitud.BORRADOR;
    public string? BloqueadoPor { get; private set; }
    public DateTime? BloqueadoDesde { get; private set; }
    public List<Partida> Partidas { get; private set; } = new();

    public decimal TotalMonto => Partidas.Sum(p => (decimal)p.Cantidad * p.PrecioUnitario);

    public bool ExcedeLimitePermitido() => TotalMonto > LimitesNegocio.MONTO_MAXIMO_SOLICITUD;

    public bool PuedeSerEditadaPor(string usuarioId)
    {
        if (string.IsNullOrEmpty(BloqueadoPor)) return true;

        if (BloqueadoDesde.HasValue &&
            DateTime.UtcNow > BloqueadoDesde.Value.AddMinutes(LimitesNegocio.MINUTOS_EXPIRACION_BLOQUEO))
            return true;

        return BloqueadoPor == usuarioId;
    }

    public void BloquearParaEdicion(string usuarioId)
    {
        BloqueadoPor = usuarioId;
        BloqueadoDesde = DateTime.UtcNow;
    }

    public void LiberarBloqueo()
    {
        BloqueadoPor = null;
        BloqueadoDesde = null;
    }

    public void CambiarAEstado(EstadoSolicitud nuevoEstado)
    {
        Estado = nuevoEstado;
    }

    public void AgregarPartida(Partida partida)
    {
        Partidas.Add(partida);
    }

    public void RemoverPartida(int partidaId)
    {
        var partida = Partidas.FirstOrDefault(p => p.Id == partidaId);
        if (partida != null)
        {
            Partidas.Remove(partida);
        }
    }
}
