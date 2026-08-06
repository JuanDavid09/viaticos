using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viaticos.Domain.Documentos.Entities;
using Viaticos.Infrastructure.Persistence.Conversions;

namespace Viaticos.Infrastructure.Persistence.Configurations;

internal class ArchivoConfiguration : IEntityTypeConfiguration<Archivo>
{
    public void Configure(EntityTypeBuilder<Archivo> builder)
    {
        builder.ToTable("archivo", "docs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Bucket).HasColumnName("bucket").HasMaxLength(100);
        builder.Property(a => a.ObjectKey).HasColumnName("object_key").HasMaxLength(500);
        builder.Property(a => a.NombreOriginal).HasColumnName("nombre_original").HasMaxLength(255);
        builder.Property(a => a.MimeType).HasColumnName("mime_type").HasMaxLength(100);
        builder.Property(a => a.TamanoBytes).HasColumnName("tamano_bytes");
        builder.Property(a => a.SubidoPor).HasColumnName("subido_por");
        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
    }
}

internal class OcrExtraccionConfiguration : IEntityTypeConfiguration<OcrExtraccion>
{
    public void Configure(EntityTypeBuilder<OcrExtraccion> builder)
    {
        builder.ToTable("ocr_extraccion", "docs");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.ArchivoId).HasColumnName("archivo_id");
        builder.Property(o => o.GastoSoporteId).HasColumnName("gasto_soporte_id");
        builder.Property(o => o.AzureOperationId).HasColumnName("azure_operation_id").HasMaxLength(100);
        builder.Property(o => o.Estado).HasColumnName("estado").HasPostgresEnum();
        builder.Property(o => o.JsonRespuesta).HasColumnName("json_respuesta").HasColumnType("jsonb");
        builder.Property(o => o.ErrorMensaje).HasColumnName("error_mensaje");
        builder.Property(o => o.ProcesadoAt).HasColumnName("procesado_at");

        builder.Property<DateTime>("CreatedAt").HasColumnName("created_at");

        builder.HasMany(o => o.Campos)
            .WithOne()
            .HasForeignKey(c => c.OcrExtraccionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Campos)
            .HasField("_campos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal class OcrCampoConfiguration : IEntityTypeConfiguration<OcrCampo>
{
    public void Configure(EntityTypeBuilder<OcrCampo> builder)
    {
        builder.ToTable("ocr_campo", "docs");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.OcrExtraccionId).HasColumnName("ocr_extraccion_id");
        builder.Property(c => c.NombreCampo).HasColumnName("nombre_campo").HasMaxLength(100);
        builder.Property(c => c.ValorExtraido).HasColumnName("valor_extraido");
        builder.Property(c => c.ValorValidado).HasColumnName("valor_validado");
        builder.Property(c => c.Validado).HasColumnName("validado");
        builder.Property(c => c.ValidadoPor).HasColumnName("validado_por");
        builder.Property(c => c.ValidadoAt).HasColumnName("validado_at");
    }
}
