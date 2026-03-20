using HCG.FondoRevolvente.Domain.Entities;

namespace HCG.FondoRevolvente.Application.Interfaces;

public interface ISolicitudRepository
{
    Task<Solicitud?> ObtenerPorIdAsync(int id);
    Task<List<Solicitud>> ObtenerTodasAsync();
    Task AgregarAsync(Solicitud solicitud);
    Task ActualizarAsync(Solicitud solicitud);
    Task<bool> ExisteAsync(int id);
}

public interface IProveedorRepository
{
    Task<Proveedor?> GetByIdAsync(int id);
    Task<IEnumerable<Proveedor>> GetAllAsync();
    Task AddAsync(Proveedor proveedor);
    Task UpdateAsync(Proveedor proveedor);
}
