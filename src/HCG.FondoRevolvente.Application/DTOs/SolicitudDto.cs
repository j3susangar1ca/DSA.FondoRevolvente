using HCG.FondoRevolvente.Domain.Enums;

namespace HCG.FondoRevolvente.Application.DTOs;

public class SolicitudDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string ConceptoGeneral { get; set; } = string.Empty;
    public string Justificacion { get; set; } = string.Empty;
    public string AreaSolicitante { get; set; } = string.Empty;
    public string NombreResponsable { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaRequerida { get; set; }
    public EstadoSolicitud Estado { get; set; }
    public decimal TotalMonto { get; set; }
}

public class CreateSolicitudDto
{
    public string ConceptoGeneral { get; set; } = string.Empty;
    public string Justificacion { get; set; } = string.Empty;
    public string AreaSolicitante { get; set; } = string.Empty;
    public string NombreResponsable { get; set; } = string.Empty;
    public DateTime? FechaRequerida { get; set; }
    public List<CreatePartidaDto> Partidas { get; set; } = new();
}

public class CreatePartidaDto
{
    public string CodigoProducto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
