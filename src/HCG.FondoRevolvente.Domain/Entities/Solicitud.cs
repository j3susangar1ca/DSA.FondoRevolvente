using System.Collections.Generic;
using System.Linq;
using HCG.FondoRevolvente.Domain.Enums;
using HCG.FondoRevolvente.Domain.Constants;

namespace HCG.FondoRevolvente.Domain.Entities;

public class Solicitud
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string ConceptoGeneral { get; set; } = string.Empty;
    public string Justificacion { get; set; } = string.Empty;
    public string AreaSolicitante { get; set; } = string.Empty;
    public string NombreResponsable { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaRequerida { get; set; }

    // --- 3. ESTADOS SIMPLIFICADOS ---
    public EstadoSolicitud Estado { get; private set; } = EstadoSolicitud.BORRADOR;

    // --- 1. REGLA DE BLOQUEO (RN-005) ---
    public string? BloqueadoPor { get; private set; }
    public DateTime? BloqueadoDesde { get; private set; }
    public List<Partida> Partidas { get; private set; } = new();
    public List<Cotizacion> Cotizaciones { get; private set; } = new();

    // --- 2. VALIDACIÓN FINANCIERA (RN-001) ---
    public decimal TotalMonto => Partidas.Sum(p => (decimal)p.Cantidad * p.PrecioUnitario);

    public bool ExcedeLimitePermitido() => TotalMonto > LimitesNegocio.MONTO_MAXIMO_SOLICITUD;

    // --- LÓGICA PRAGMÁTICA DE BLOQUEO ---
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

    // Métodos para transición de estados
    public void CambiarAEstado(EstadoSolicitud nuevoEstado)
    {
        Estado = nuevoEstado;
    }
}
