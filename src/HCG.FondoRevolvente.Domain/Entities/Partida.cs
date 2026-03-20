namespace HCG.FondoRevolvente.Domain.Entities;

public class Partida
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    
    // As per the previous implementation, adding Description and Code might be useful
    public string CodigoProducto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public decimal Subtotal => Cantidad * PrecioUnitario;
}
