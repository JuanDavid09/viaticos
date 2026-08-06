using Microsoft.EntityFrameworkCore;
using Viaticos.Domain.Core.Entities;
using Viaticos.Domain.Documentos.Entities;
using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Infrastructure.Persistence;

public class ViaticosDbContext : DbContext
{
    public ViaticosDbContext(DbContextOptions<ViaticosDbContext> options) : base(options)
    {
    }

    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Moneda> Monedas => Set<Moneda>();
    public DbSet<CategoriaGasto> CategoriasGasto => Set<CategoriaGasto>();
    public DbSet<Legalizacion> Legalizaciones => Set<Legalizacion>();
    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<LegalizacionHistorial> LegalizacionHistorial => Set<LegalizacionHistorial>();
    public DbSet<Archivo> Archivos => Set<Archivo>();
    public DbSet<OcrExtraccion> OcrExtracciones => Set<OcrExtraccion>();
    public DbSet<OcrCampo> OcrCampos => Set<OcrCampo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ViaticosDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    private void ApplyTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Metadata.FindProperty("CreatedAt") is not null)
                    entry.Property("CreatedAt").CurrentValue = now;

                if (entry.Metadata.FindProperty("UpdatedAt") is not null)
                    entry.Property("UpdatedAt").CurrentValue = now;
            }

            if (entry.State == EntityState.Modified && entry.Metadata.FindProperty("UpdatedAt") is not null)
                entry.Property("UpdatedAt").CurrentValue = now;
        }
    }
}
