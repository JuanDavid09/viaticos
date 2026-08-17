using Viaticos.Domain.Legalizaciones.Enums;

namespace Viaticos.Infrastructure.Persistence.Repositories;

internal static class ReporteSqlMapper
{
    public static string MapEstado(string? dbValue)
    {
        if (string.IsNullOrWhiteSpace(dbValue))
            return string.Empty;

        return dbValue.Trim().ToUpperInvariant() switch
        {
            "BORRADOR" => nameof(EstadoLegalizacion.Borrador),
            "PENDIENTE_VALIDACION" => nameof(EstadoLegalizacion.PendienteValidacion),
            "PENDIENTE_APROBACION" => nameof(EstadoLegalizacion.PendienteAprobacion),
            "APROBADA" => nameof(EstadoLegalizacion.Aprobada),
            "RECHAZADA" => nameof(EstadoLegalizacion.Rechazada),
            "PENDIENTE_NOMINA" => nameof(EstadoLegalizacion.PendienteNomina),
            "CERRADA" => nameof(EstadoLegalizacion.Cerrada),
            _ => dbValue
        };
    }

    public static string? MapEstadoToDb(EstadoLegalizacion? estado) =>
        estado switch
        {
            null => null,
            EstadoLegalizacion.Borrador => "BORRADOR",
            EstadoLegalizacion.PendienteValidacion => "PENDIENTE_VALIDACION",
            EstadoLegalizacion.PendienteAprobacion => "PENDIENTE_APROBACION",
            EstadoLegalizacion.Aprobada => "APROBADA",
            EstadoLegalizacion.Rechazada => "RECHAZADA",
            EstadoLegalizacion.PendienteNomina => "PENDIENTE_NOMINA",
            EstadoLegalizacion.Cerrada => "CERRADA",
            _ => null
        };

    public static string? GetStringOrNull(Npgsql.NpgsqlDataReader reader, string column) =>
        reader.IsDBNull(reader.GetOrdinal(column)) ? null : reader.GetString(reader.GetOrdinal(column));

    public static decimal GetDecimal(Npgsql.NpgsqlDataReader reader, string column) =>
        reader.GetDecimal(reader.GetOrdinal(column));

    public static long GetInt64(Npgsql.NpgsqlDataReader reader, string column) =>
        reader.GetInt64(reader.GetOrdinal(column));

    public static int GetInt32(Npgsql.NpgsqlDataReader reader, string column) =>
        reader.GetInt32(reader.GetOrdinal(column));

    public static Guid GetGuid(Npgsql.NpgsqlDataReader reader, string column) =>
        reader.GetGuid(reader.GetOrdinal(column));

    public static DateOnly GetDateOnly(Npgsql.NpgsqlDataReader reader, string column) =>
        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal(column)));

    public static DateTime GetDateTime(Npgsql.NpgsqlDataReader reader, string column) =>
        reader.GetDateTime(reader.GetOrdinal(column));

    public static DateTime? GetDateTimeOrNull(Npgsql.NpgsqlDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }

    public static int? GetInt32OrNull(Npgsql.NpgsqlDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    }

    public static bool GetBoolean(Npgsql.NpgsqlDataReader reader, string column) =>
        reader.GetBoolean(reader.GetOrdinal(column));
}
