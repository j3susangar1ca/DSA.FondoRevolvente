using HCG.FondoRevolvente.Domain.Enums;

namespace HCG.FondoRevolvente.Domain.Entities;

public class Solicitud
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty; // DSA-YYYY-NNN
    public string ConceptoGeneral { get; set; } = string.Empty;
    public string Justificacion { get; set; } = string.Empty;
    public string AreaSolicitante { get; set; } = string.Empty;
    public string NombreResponsable { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaRequerida { get; set; }
    
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.BORRADOR;
    
    // RN-005: Locking mechanism
    public string? BloqueadoPor { get; set; }
    public DateTime? BloqueadoDesde { get; set; }
    
    public List<Partida> Partidas { get; set; } = new();
    public List<Cotizacion> Cotizaciones { get; set; } = new();

    public decimal TotalMonto => Partidas.Sum(p => p.Subtotal);
}
