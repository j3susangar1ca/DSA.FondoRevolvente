using Microsoft.EntityFrameworkCore;
using HCG.FondoRevolvente.Application.Interfaces;
using HCG.FondoRevolvente.Domain.Entities;
using HCG.FondoRevolvente.Infrastructure.Data;

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
        // El .Include es VITAL para que calcule el TotalMonto (RN-001) correctamente
        return await _context.Solicitudes
            .Include(s => s.Partidas) 
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Solicitud>> ObtenerTodasAsync()
    {
        return await _context.Solicitudes.ToListAsync();
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
