using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viaticos.Domain.Core.Entities;
using Viaticos.Infrastructure.Persistence.Conversions;

namespace Viaticos.Infrastructure.Persistence.Configurations;

internal class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> builder)
    {
        builder.ToTable("empleado", "core");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CodigoEmpleado).HasColumnName("codigo_empleado").HasMaxLength(30);
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(254);
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100);
        builder.Property(e => e.Apellido).HasColumnName("apellido").HasMaxLength(100);
        builder.Property(e => e.Departamento).HasColumnName("departamento").HasMaxLength(100);
        builder.Property(e => e.Rol).HasColumnName("rol").HasPostgresEnum();
        builder.Property(e => e.JefeId).HasColumnName("jefe_id");
        builder.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
        builder.Property(e => e.MustChangePassword).HasColumnName("must_change_password");
        builder.Property(e => e.Activo).HasColumnName("activo");

        builder.Property<DateTime>("CreatedAt").HasColumnName("created_at");
        builder.Property<DateTime>("UpdatedAt").HasColumnName("updated_at");
    }
}

internal class MonedaConfiguration : IEntityTypeConfiguration<Moneda>
{
    public void Configure(EntityTypeBuilder<Moneda> builder)
    {
        builder.ToTable("moneda", "core");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.CodigoIso).HasColumnName("codigo_iso").HasMaxLength(3);
        builder.Property(m => m.Nombre).HasColumnName("nombre").HasMaxLength(50);
        builder.Property(m => m.Simbolo).HasColumnName("simbolo").HasMaxLength(5);
        builder.Property(m => m.Activo).HasColumnName("activo");
    }
}

internal class CategoriaGastoConfiguration : IEntityTypeConfiguration<CategoriaGasto>
{
    public void Configure(EntityTypeBuilder<CategoriaGasto> builder)
    {
        builder.ToTable("categoria_gasto", "core");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Codigo).HasColumnName("codigo").HasMaxLength(30);
        builder.Property(c => c.Nombre).HasColumnName("nombre").HasMaxLength(100);
        builder.Property(c => c.RequiereSoporte).HasColumnName("requiere_soporte");
        builder.Property(c => c.Activo).HasColumnName("activo");
    }
}
