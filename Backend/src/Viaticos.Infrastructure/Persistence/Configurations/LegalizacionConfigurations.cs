using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viaticos.Domain.Legalizaciones.Entities;
using Viaticos.Infrastructure.Persistence.Conversions;

namespace Viaticos.Infrastructure.Persistence.Configurations;

internal class LegalizacionConfiguration : IEntityTypeConfiguration<Legalizacion>
{
    public void Configure(EntityTypeBuilder<Legalizacion> builder)
    {
        builder.ToTable("legalizacion", "viaticos");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Numero).HasColumnName("numero").HasMaxLength(30);
        builder.Property(l => l.EmpleadoId).HasColumnName("empleado_id");
        builder.Property(l => l.Motivo).HasColumnName("motivo");
        builder.Property(l => l.Destino).HasColumnName("destino").HasMaxLength(200);
        builder.Property(l => l.FechaInicio).HasColumnName("fecha_inicio");
        builder.Property(l => l.FechaFin).HasColumnName("fecha_fin");
        builder.Property(l => l.MonedaId).HasColumnName("moneda_id");
        builder.Property(l => l.MontoAnticipo).HasColumnName("monto_anticipo").HasPrecision(18, 2);
        builder.Property(l => l.Estado).HasColumnName("estado").HasPostgresEnum();
        builder.Property(l => l.TotalGastos).HasColumnName("total_gastos").HasPrecision(18, 2);
        builder.Property(l => l.TotalReembolso).HasColumnName("total_reembolso").HasPrecision(18, 2);
        builder.Property(l => l.TotalDevolucion).HasColumnName("total_devolucion").HasPrecision(18, 2);
        builder.Property(l => l.Observaciones).HasColumnName("observaciones");
        builder.Property(l => l.DeletedAt).HasColumnName("deleted_at");
        builder.Property(l => l.CreatedBy).HasColumnName("created_by");
        builder.Property(l => l.UpdatedBy).HasColumnName("updated_by");
        builder.Property(l => l.SubmittedAt).HasColumnName("submitted_at");
        builder.Property(l => l.ClosedAt).HasColumnName("closed_at");
        builder.Property(l => l.CreatedAt).HasColumnName("created_at");

        builder.Property<DateTime>("UpdatedAt").HasColumnName("updated_at");

        builder.HasMany(l => l.Gastos)
            .WithOne()
            .HasForeignKey(g => g.LegalizacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(l => l.Gastos)
            .HasField("_gastos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(l => l.DeletedAt == null);
    }
}

internal class GastoConfiguration : IEntityTypeConfiguration<Gasto>
{
    public void Configure(EntityTypeBuilder<Gasto> builder)
    {
        builder.ToTable("gasto", "viaticos");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.LegalizacionId).HasColumnName("legalizacion_id");
        builder.Property(g => g.CategoriaGastoId).HasColumnName("categoria_gasto_id");
        builder.Property(g => g.FechaGasto).HasColumnName("fecha_gasto");
        builder.Property(g => g.Descripcion).HasColumnName("descripcion");
        builder.Property(g => g.Proveedor).HasColumnName("proveedor").HasMaxLength(200);
        builder.Property(g => g.NumeroDocumento).HasColumnName("numero_documento").HasMaxLength(50);
        builder.Property(g => g.Monto).HasColumnName("monto").HasPrecision(18, 2);
        builder.Property(g => g.Validado).HasColumnName("validado");
        builder.Property(g => g.ValidadoPor).HasColumnName("validado_por");
        builder.Property(g => g.ValidadoAt).HasColumnName("validado_at");
        builder.Property(g => g.Orden).HasColumnName("orden");
        builder.Property(g => g.DeletedAt).HasColumnName("deleted_at");
        builder.Property(g => g.CreatedBy).HasColumnName("created_by");

        builder.Property<DateTime>("CreatedAt").HasColumnName("created_at");
        builder.Property<DateTime>("UpdatedAt").HasColumnName("updated_at");

        builder.HasQueryFilter(g => g.DeletedAt == null);
    }
}

internal class LegalizacionHistorialConfiguration : IEntityTypeConfiguration<LegalizacionHistorial>
{
    public void Configure(EntityTypeBuilder<LegalizacionHistorial> builder)
    {
        builder.ToTable("legalizacion_historial", "viaticos");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.LegalizacionId).HasColumnName("legalizacion_id");
        builder.Property(h => h.EstadoAnterior).HasColumnName("estado_anterior").HasPostgresEnum();
        builder.Property(h => h.EstadoNuevo).HasColumnName("estado_nuevo").HasPostgresEnum();
        builder.Property(h => h.UsuarioId).HasColumnName("usuario_id");
        builder.Property(h => h.Comentario).HasColumnName("comentario");
        builder.Property(h => h.CreatedAt).HasColumnName("created_at");
    }
}
