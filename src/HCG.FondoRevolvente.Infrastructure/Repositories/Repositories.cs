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

    public async Task<Solicitud?> GetByIdAsync(int id)
    {
        return await _context.Solicitudes
            .Include(s => s.Partidas)
            .Include(s => s.Cotizaciones)
            .ThenInclude(c => c.Proveedor)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Solicitud>> GetAllAsync()
    {
        return await _context.Solicitudes
            .Include(s => s.Partidas)
            .ToListAsync();
    }

    public async Task AddAsync(Solicitud solicitud)
    {
        await _context.Solicitudes.AddAsync(solicitud);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Solicitud solicitud)
    {
        _context.Solicitudes.Update(solicitud);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var solicitud = await _context.Solicitudes.FindAsync(id);
        if (solicitud != null)
        {
            _context.Solicitudes.Remove(solicitud);
            await _context.SaveChangesAsync();
        }
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
