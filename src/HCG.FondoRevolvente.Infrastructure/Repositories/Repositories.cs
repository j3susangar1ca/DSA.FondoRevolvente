using HCG.FondoRevolvente.Application.Interfaces;
using HCG.FondoRevolvente.Domain.Entities;
using HCG.FondoRevolvente.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HCG.FondoRevolvente.Infrastructure.Repositories;

public class SolicitudRepository : ISolicitudRepository
{
    private readonly AppDbContext _context;

    public SolicitudRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Solicitud?> ObtenerPorIdAsync(int id)
    {
        return await _context.Solicitudes
            .Include(s => s.Partidas)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Solicitud>> ObtenerTodasAsync()
    {
        return await _context.Solicitudes
            .Include(s => s.Partidas)
            .ToListAsync();
    }

    public async Task AgregarAsync(Solicitud solicitud)
    {
        await _context.Solicitudes.AddAsync(solicitud);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Solicitud solicitud)
    {
        _context.Solicitudes.Update(solicitud);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Solicitudes.AnyAsync(s => s.Id == id);
    }
}

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
