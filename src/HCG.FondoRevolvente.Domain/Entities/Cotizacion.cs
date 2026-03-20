namespace HCG.FondoRevolvente.Domain.Entities;

public class Cotizacion
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }
    public decimal MontoTotal { get; set; }
    public int DiasEntrega { get; set; }
    public string Condiciones { get; set; } = string.Empty;
    public string ArchivoAdjuntoUri { get; set; } = string.Empty;
    public bool Seleccionada { get; set; }
}
