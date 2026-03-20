using HCG.FondoRevolvente.Application.Interfaces;
using HCG.FondoRevolvente.Domain.Entities;
using HCG.FondoRevolvente.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HCG.FondoRevolvente.Infrastructure.Repositories;

public class ProveedorRepository : IProveedorRepository
{
    private readonly AppDbContext _context;

    public ProveedorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Proveedor?> GetByIdAsync(int id)
    {
        return await _context.Proveedores.FindAsync(id);
    }

    public async Task<IEnumerable<Proveedor>> GetAllAsync()
    {
        return await _context.Proveedores.ToListAsync();
    }

    public async Task AddAsync(Proveedor proveedor)
    {
        await _context.Proveedores.AddAsync(proveedor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Proveedor proveedor)
    {
        _context.Proveedores.Update(proveedor);
        await _context.SaveChangesAsync();
    }
}
