namespace HCG.FondoRevolvente.Domain.Enums;

public enum FaseProceso
{
    Borrador = 1,
    RevisionCAA = 2,
    Cotizacion = 3,
    SeleccionProveedor = 4,
    Entrega = 5,
    ValidacionFiscal = 6,
    Cierre = 7
}

public enum EstadoSolicitud
{
    BORRADOR = 1,
    EN_REVISION_CAA = 2,
    RECHAZADO_CAA = 3,
    COTIZANDO = 4,
    PROVEEDOR_SELECCIONADO = 5,
    ENTREGADO = 6,
    CFDI_VALIDADO = 7,
    PAGADO = 8,
    CANCELADO = 9
}

public enum RolAplicacion
{
    Administrador = 1,
    CompradorDSA = 2,
    RevisorCAA = 3,
    Finanzas = 4,
    Almacen = 5,
    ConsultaDSA = 6
}
