using Npgsql;
using NpgsqlTypes;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Reportes;

namespace Viaticos.Infrastructure.Persistence.Repositories;

public class ReporteRepository : IReporteRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ReporteRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public Task<IReadOnlyList<ResumenPorEstadoDto>> GetResumenPorEstadoAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            """
            SELECT *
            FROM reportes.sp_resumen_por_estado(
                @p_fecha_desde,
                @p_fecha_hasta,
                @p_empleado_id,
                @p_jefe_id,
                @p_departamento
            )
            """,
            cmd => AddCommonFilters(cmd, filtros),
            reader => new ResumenPorEstadoDto(
                ReporteSqlMapper.MapEstado(reader.GetString(reader.GetOrdinal("estado"))),
                ReporteSqlMapper.GetInt64(reader, "cantidad"),
                ReporteSqlMapper.GetDecimal(reader, "total_anticipos"),
                ReporteSqlMapper.GetDecimal(reader, "total_gastos"),
                ReporteSqlMapper.GetDecimal(reader, "total_reembolsos"),
                ReporteSqlMapper.GetDecimal(reader, "total_devoluciones")),
            cancellationToken);

    public Task<IReadOnlyList<LegalizacionDetalleReporteDto>> GetLegalizacionesDetalleAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            """
            SELECT *
            FROM reportes.sp_legalizaciones_detalle(
                @p_fecha_desde,
                @p_fecha_hasta,
                @p_empleado_id,
                @p_jefe_id,
                @p_departamento,
                @p_estado
            )
            """,
            cmd =>
            {
                AddCommonFilters(cmd, filtros);
                AddEstadoParameter(cmd, filtros.Estado);
            },
            reader => new LegalizacionDetalleReporteDto(
                ReporteSqlMapper.GetGuid(reader, "id"),
                reader.GetString(reader.GetOrdinal("numero")),
                reader.GetString(reader.GetOrdinal("empleado_codigo")),
                reader.GetString(reader.GetOrdinal("empleado_nombre")),
                ReporteSqlMapper.GetStringOrNull(reader, "departamento"),
                ReporteSqlMapper.GetStringOrNull(reader, "jefe_nombre"),
                reader.GetString(reader.GetOrdinal("motivo")),
                ReporteSqlMapper.GetStringOrNull(reader, "destino"),
                ReporteSqlMapper.GetDateOnly(reader, "fecha_inicio"),
                ReporteSqlMapper.GetDateOnly(reader, "fecha_fin"),
                reader.GetString(reader.GetOrdinal("moneda_codigo")).Trim(),
                ReporteSqlMapper.GetStringOrNull(reader, "moneda_simbolo"),
                ReporteSqlMapper.GetDecimal(reader, "monto_anticipo"),
                ReporteSqlMapper.MapEstado(reader.GetString(reader.GetOrdinal("estado"))),
                ReporteSqlMapper.GetDecimal(reader, "total_gastos"),
                ReporteSqlMapper.GetDecimal(reader, "total_reembolso"),
                ReporteSqlMapper.GetDecimal(reader, "total_devolucion"),
                ReporteSqlMapper.GetDecimal(reader, "saldo_anticipo"),
                ReporteSqlMapper.GetDateTime(reader, "created_at"),
                ReporteSqlMapper.GetDateTimeOrNull(reader, "submitted_at"),
                ReporteSqlMapper.GetDateTimeOrNull(reader, "closed_at")),
            cancellationToken);

    public Task<IReadOnlyList<GastoPorCategoriaDto>> GetGastosPorCategoriaAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            """
            SELECT *
            FROM reportes.sp_gastos_por_categoria(
                @p_fecha_desde,
                @p_fecha_hasta,
                @p_empleado_id,
                @p_jefe_id,
                @p_departamento,
                @p_estado
            )
            """,
            cmd =>
            {
                AddCommonFilters(cmd, filtros);
                AddEstadoParameter(cmd, filtros.Estado);
            },
            reader => new GastoPorCategoriaDto(
                reader.GetString(reader.GetOrdinal("categoria_codigo")),
                reader.GetString(reader.GetOrdinal("categoria_nombre")),
                ReporteSqlMapper.GetInt64(reader, "cantidad_gastos"),
                ReporteSqlMapper.GetDecimal(reader, "total_monto"),
                ReporteSqlMapper.GetDecimal(reader, "promedio_monto")),
            cancellationToken);

    public Task<IReadOnlyList<GastoDetalleReporteDto>> GetGastosDetalleAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            """
            SELECT *
            FROM reportes.sp_gastos_detalle(
                @p_fecha_desde,
                @p_fecha_hasta,
                @p_empleado_id,
                @p_jefe_id,
                NULL,
                @p_estado
            )
            """,
            cmd =>
            {
                AddCommonFilters(cmd, filtros);
                AddEstadoParameter(cmd, filtros.Estado);
            },
            reader => new GastoDetalleReporteDto(
                reader.GetString(reader.GetOrdinal("legalizacion_numero")),
                ReporteSqlMapper.MapEstado(reader.GetString(reader.GetOrdinal("legalizacion_estado"))),
                reader.GetString(reader.GetOrdinal("empleado_codigo")),
                reader.GetString(reader.GetOrdinal("empleado_nombre")),
                ReporteSqlMapper.GetStringOrNull(reader, "departamento"),
                reader.GetString(reader.GetOrdinal("moneda_codigo")).Trim(),
                ReporteSqlMapper.GetGuid(reader, "gasto_id"),
                reader.GetString(reader.GetOrdinal("categoria_codigo")),
                reader.GetString(reader.GetOrdinal("categoria_nombre")),
                ReporteSqlMapper.GetDateOnly(reader, "fecha_gasto"),
                reader.GetString(reader.GetOrdinal("descripcion")),
                ReporteSqlMapper.GetStringOrNull(reader, "proveedor"),
                ReporteSqlMapper.GetStringOrNull(reader, "numero_documento"),
                ReporteSqlMapper.GetDecimal(reader, "monto"),
                ReporteSqlMapper.GetBoolean(reader, "validado"),
                ReporteSqlMapper.GetInt64(reader, "cantidad_soportes")),
            cancellationToken);

    public Task<IReadOnlyList<ResumenFinancieroEmpleadoDto>> GetResumenFinancieroEmpleadoAsync(
        ReporteFiltros filtros,
        bool soloCerradas,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            """
            SELECT *
            FROM reportes.sp_resumen_financiero_empleado(
                @p_fecha_desde,
                @p_fecha_hasta,
                @p_jefe_id,
                @p_departamento,
                @p_solo_cerradas
            )
            """,
            cmd =>
            {
                AddDateFilters(cmd, filtros);
                AddNullableGuid(cmd, "@p_jefe_id", filtros.JefeId);
                AddNullableString(cmd, "@p_departamento", filtros.Departamento);
                cmd.Parameters.AddWithValue("@p_solo_cerradas", soloCerradas);
            },
            reader => new ResumenFinancieroEmpleadoDto(
                ReporteSqlMapper.GetGuid(reader, "empleado_id"),
                reader.GetString(reader.GetOrdinal("empleado_codigo")),
                reader.GetString(reader.GetOrdinal("empleado_nombre")),
                ReporteSqlMapper.GetStringOrNull(reader, "departamento"),
                reader.GetString(reader.GetOrdinal("moneda_codigo")).Trim(),
                ReporteSqlMapper.GetInt64(reader, "cantidad_legalizaciones"),
                ReporteSqlMapper.GetDecimal(reader, "total_anticipos"),
                ReporteSqlMapper.GetDecimal(reader, "total_gastos"),
                ReporteSqlMapper.GetDecimal(reader, "total_reembolsos"),
                ReporteSqlMapper.GetDecimal(reader, "total_devoluciones")),
            cancellationToken);

    public Task<IReadOnlyList<PendienteAprobacionReporteDto>> GetPendientesAprobacionAsync(
        Guid? jefeId,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            "SELECT * FROM reportes.sp_pendientes_aprobacion(@p_jefe_id)",
            cmd => AddNullableGuid(cmd, "@p_jefe_id", jefeId),
            reader => new PendienteAprobacionReporteDto(
                ReporteSqlMapper.GetGuid(reader, "id"),
                reader.GetString(reader.GetOrdinal("numero")),
                reader.GetString(reader.GetOrdinal("empleado_codigo")),
                reader.GetString(reader.GetOrdinal("empleado_nombre")),
                ReporteSqlMapper.GetStringOrNull(reader, "departamento"),
                reader.GetString(reader.GetOrdinal("motivo")),
                ReporteSqlMapper.GetStringOrNull(reader, "destino"),
                ReporteSqlMapper.GetDateOnly(reader, "fecha_inicio"),
                ReporteSqlMapper.GetDateOnly(reader, "fecha_fin"),
                reader.GetString(reader.GetOrdinal("moneda_codigo")).Trim(),
                ReporteSqlMapper.GetDecimal(reader, "monto_anticipo"),
                ReporteSqlMapper.GetDecimal(reader, "total_gastos"),
                ReporteSqlMapper.GetDateTimeOrNull(reader, "submitted_at"),
                ReporteSqlMapper.GetInt32OrNull(reader, "dias_pendientes")),
            cancellationToken);

    public Task<IReadOnlyList<PendienteNominaReporteDto>> GetPendientesNominaAsync(
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            "SELECT * FROM reportes.sp_pendientes_nomina()",
            _ => { },
            reader => new PendienteNominaReporteDto(
                ReporteSqlMapper.GetGuid(reader, "id"),
                reader.GetString(reader.GetOrdinal("numero")),
                reader.GetString(reader.GetOrdinal("empleado_codigo")),
                reader.GetString(reader.GetOrdinal("empleado_nombre")),
                ReporteSqlMapper.GetStringOrNull(reader, "departamento"),
                reader.GetString(reader.GetOrdinal("motivo")),
                reader.GetString(reader.GetOrdinal("moneda_codigo")).Trim(),
                ReporteSqlMapper.GetDecimal(reader, "monto_anticipo"),
                ReporteSqlMapper.GetDecimal(reader, "total_gastos"),
                ReporteSqlMapper.GetDecimal(reader, "total_reembolso"),
                ReporteSqlMapper.GetDecimal(reader, "total_devolucion"),
                ReporteSqlMapper.GetDateTimeOrNull(reader, "submitted_at")),
            cancellationToken);

    public Task<IReadOnlyList<LegalizacionCerradaReporteDto>> GetLegalizacionesCerradasAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            """
            SELECT *
            FROM reportes.sp_legalizaciones_cerradas(
                @p_fecha_desde,
                @p_fecha_hasta,
                @p_empleado_id,
                @p_jefe_id,
                @p_departamento
            )
            """,
            cmd => AddCommonFilters(cmd, filtros),
            reader => new LegalizacionCerradaReporteDto(
                ReporteSqlMapper.GetGuid(reader, "id"),
                reader.GetString(reader.GetOrdinal("numero")),
                reader.GetString(reader.GetOrdinal("empleado_codigo")),
                reader.GetString(reader.GetOrdinal("empleado_nombre")),
                ReporteSqlMapper.GetStringOrNull(reader, "departamento"),
                reader.GetString(reader.GetOrdinal("moneda_codigo")).Trim(),
                ReporteSqlMapper.GetDecimal(reader, "monto_anticipo"),
                ReporteSqlMapper.GetDecimal(reader, "total_gastos"),
                ReporteSqlMapper.GetDecimal(reader, "total_reembolso"),
                ReporteSqlMapper.GetDecimal(reader, "total_devolucion"),
                ReporteSqlMapper.GetDateTimeOrNull(reader, "closed_at")),
            cancellationToken);

    public Task<IReadOnlyList<GastoSinSoporteDto>> GetGastosSinSoporteAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            """
            SELECT *
            FROM reportes.sp_gastos_sin_soporte(
                @p_fecha_desde,
                @p_fecha_hasta,
                @p_estado
            )
            """,
            cmd =>
            {
                AddDateFilters(cmd, filtros);
                AddEstadoParameter(cmd, filtros.Estado);
            },
            reader => new GastoSinSoporteDto(
                reader.GetString(reader.GetOrdinal("legalizacion_numero")),
                ReporteSqlMapper.MapEstado(reader.GetString(reader.GetOrdinal("legalizacion_estado"))),
                reader.GetString(reader.GetOrdinal("empleado_nombre")),
                reader.GetString(reader.GetOrdinal("categoria_nombre")),
                ReporteSqlMapper.GetDateOnly(reader, "fecha_gasto"),
                reader.GetString(reader.GetOrdinal("descripcion")),
                ReporteSqlMapper.GetDecimal(reader, "monto"),
                ReporteSqlMapper.GetBoolean(reader, "requiere_soporte")),
            cancellationToken);

    public Task<IReadOnlyList<HistorialAuditoriaDto>> GetHistorialAuditoriaAsync(
        ReporteFiltros filtros,
        Guid? legalizacionId,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            """
            SELECT *
            FROM reportes.sp_historial_auditoria(
                @p_fecha_desde,
                @p_fecha_hasta,
                @p_legalizacion_id,
                @p_empleado_id
            )
            """,
            cmd =>
            {
                AddDateFilters(cmd, filtros);
                AddNullableGuid(cmd, "@p_legalizacion_id", legalizacionId);
                AddNullableGuid(cmd, "@p_empleado_id", filtros.EmpleadoId);
            },
            reader => new HistorialAuditoriaDto(
                ReporteSqlMapper.GetGuid(reader, "historial_id"),
                reader.GetString(reader.GetOrdinal("legalizacion_numero")),
                reader.GetString(reader.GetOrdinal("empleado_nombre")),
                MapEstadoOrNull(ReporteSqlMapper.GetStringOrNull(reader, "estado_anterior")),
                ReporteSqlMapper.MapEstado(reader.GetString(reader.GetOrdinal("estado_nuevo"))),
                reader.GetString(reader.GetOrdinal("usuario_nombre")),
                ReporteSqlMapper.GetStringOrNull(reader, "comentario"),
                ReporteSqlMapper.GetDateTime(reader, "created_at")),
            cancellationToken);

    public Task<IReadOnlyList<VolumenMensualDto>> GetVolumenMensualAsync(
        ReporteFiltros filtros,
        int? anio,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            """
            SELECT *
            FROM reportes.sp_volumen_mensual(
                @p_anio,
                @p_jefe_id,
                @p_departamento
            )
            """,
            cmd =>
            {
                cmd.Parameters.AddWithValue("@p_anio", (object?)anio ?? DBNull.Value);
                AddNullableGuid(cmd, "@p_jefe_id", filtros.JefeId);
                AddNullableString(cmd, "@p_departamento", filtros.Departamento);
            },
            reader => new VolumenMensualDto(
                ReporteSqlMapper.GetInt32(reader, "anio"),
                ReporteSqlMapper.GetInt32(reader, "mes"),
                reader.GetString(reader.GetOrdinal("periodo")),
                ReporteSqlMapper.GetInt64(reader, "cantidad_legalizaciones"),
                ReporteSqlMapper.GetDecimal(reader, "total_anticipos"),
                ReporteSqlMapper.GetDecimal(reader, "total_gastos"),
                ReporteSqlMapper.GetDecimal(reader, "total_reembolsos"),
                ReporteSqlMapper.GetDecimal(reader, "total_devoluciones"),
                ReporteSqlMapper.GetInt64(reader, "cantidad_cerradas")),
            cancellationToken);

    public Task<IReadOnlyList<TiempoPorEstadoDto>> GetTiemposPorEstadoAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            """
            SELECT *
            FROM reportes.sp_tiempos_por_estado(
                @p_fecha_desde,
                @p_fecha_hasta
            )
            """,
            cmd => AddDateFilters(cmd, filtros),
            reader => new TiempoPorEstadoDto(
                reader.GetString(reader.GetOrdinal("legalizacion_numero")),
                reader.GetString(reader.GetOrdinal("empleado_nombre")),
                ReporteSqlMapper.MapEstado(reader.GetString(reader.GetOrdinal("estado"))),
                ReporteSqlMapper.GetDateTime(reader, "inicio_estado"),
                ReporteSqlMapper.GetDateTimeOrNull(reader, "fin_estado"),
                ReporteSqlMapper.GetDecimal(reader, "horas_en_estado")),
            cancellationToken);

    private async Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql,
        Action<NpgsqlCommand> configure,
        Func<NpgsqlDataReader, T> map,
        CancellationToken cancellationToken)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        configure(command);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var items = new List<T>();
        while (await reader.ReadAsync(cancellationToken))
            items.Add(map(reader));

        return items;
    }

    private static void AddCommonFilters(NpgsqlCommand command, ReporteFiltros filtros)
    {
        AddDateFilters(command, filtros);
        AddNullableGuid(command, "@p_empleado_id", filtros.EmpleadoId);
        AddNullableGuid(command, "@p_jefe_id", filtros.JefeId);
        AddNullableString(command, "@p_departamento", filtros.Departamento);
    }

    private static void AddDateFilters(NpgsqlCommand command, ReporteFiltros filtros)
    {
        command.Parameters.AddWithValue(
            "@p_fecha_desde",
            filtros.FechaDesde.HasValue ? filtros.FechaDesde.Value : DBNull.Value);
        command.Parameters.AddWithValue(
            "@p_fecha_hasta",
            filtros.FechaHasta.HasValue ? filtros.FechaHasta.Value : DBNull.Value);
    }

    private static void AddEstadoParameter(NpgsqlCommand command, Domain.Legalizaciones.Enums.EstadoLegalizacion? estado)
    {
        var dbEstado = ReporteSqlMapper.MapEstadoToDb(estado);
        var parameter = new NpgsqlParameter("@p_estado", NpgsqlDbType.Unknown)
        {
            Value = dbEstado is null ? DBNull.Value : dbEstado
        };

        if (dbEstado is not null)
            parameter.DataTypeName = "viaticos.estado_legalizacion_enum";

        command.Parameters.Add(parameter);
    }

    private static void AddNullableGuid(NpgsqlCommand command, string name, Guid? value) =>
        command.Parameters.AddWithValue(name, value.HasValue ? value.Value : DBNull.Value);

    private static void AddNullableString(NpgsqlCommand command, string name, string? value) =>
        command.Parameters.AddWithValue(name, string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim());

    private static string? MapEstadoOrNull(string? dbValue) =>
        string.IsNullOrWhiteSpace(dbValue) ? null : ReporteSqlMapper.MapEstado(dbValue);
}
