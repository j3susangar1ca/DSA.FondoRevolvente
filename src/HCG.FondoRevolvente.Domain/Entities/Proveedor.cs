namespace HCG.FondoRevolvente.Domain.Entities;

public class Proveedor
{
    public int Id { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string RFC { get; set; } = string.Empty;
    public string NombreContacto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
