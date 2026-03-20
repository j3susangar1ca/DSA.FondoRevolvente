using HCG.FondoRevolvente.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HCG.FondoRevolvente.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Solicitud> Solicitudes { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<Cotizacion> Cotizaciones { get; set; }
    public DbSet<Partida> Partidas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Solicitud>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Folio).IsRequired().HasMaxLength(20);
            entity.HasMany(e => e.Partidas).WithOne().OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Cotizaciones).WithOne().OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RazonSocial).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RFC).IsRequired().HasMaxLength(13);
        });

        modelBuilder.Entity<Cotizacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Proveedor).WithMany().HasForeignKey(e => e.ProveedorId);
        });

        modelBuilder.Entity<Partida>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)");
        });
    }
}
