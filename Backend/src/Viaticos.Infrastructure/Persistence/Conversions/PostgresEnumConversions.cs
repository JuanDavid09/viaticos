using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viaticos.Domain.Core.Enums;
using Viaticos.Domain.Documentos.Enums;
using Viaticos.Domain.Legalizaciones.Enums;

namespace Viaticos.Infrastructure.Persistence.Conversions;

internal static class PostgresEnumConversions
{
    public static PropertyBuilder<Rol> HasPostgresEnum(this PropertyBuilder<Rol> property) =>
        property.HasColumnType("core.rol_enum");

    public static PropertyBuilder<EstadoLegalizacion> HasPostgresEnum(this PropertyBuilder<EstadoLegalizacion> property) =>
        property.HasColumnType("viaticos.estado_legalizacion_enum");

    public static PropertyBuilder<EstadoLegalizacion?> HasPostgresEnum(this PropertyBuilder<EstadoLegalizacion?> property) =>
        property.HasColumnType("viaticos.estado_legalizacion_enum");

    public static PropertyBuilder<EstadoOcr> HasPostgresEnum(this PropertyBuilder<EstadoOcr> property) =>
        property.HasColumnType("docs.estado_ocr_enum");
}
