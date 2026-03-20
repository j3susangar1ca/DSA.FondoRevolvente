using HCG.FondoRevolvente.Domain.Entities;

namespace HCG.FondoRevolvente.Application.Interfaces;

public interface ISolicitudRepository
{
    Task<Solicitud?> GetByIdAsync(int id);
    Task<IEnumerable<Solicitud>> GetAllAsync();
    Task AddAsync(Solicitud solicitud);
    Task UpdateAsync(Solicitud solicitud);
    Task SaveAsync(Solicitud solicitud); // Added as per user's refinement
    Task DeleteAsync(int id);
}

public interface IProveedorRepository
{
    Task<Proveedor?> GetByIdAsync(int id);
    Task<IEnumerable<Proveedor>> GetAllAsync();
    Task AddAsync(Proveedor proveedor);
    Task UpdateAsync(Proveedor proveedor);
}
