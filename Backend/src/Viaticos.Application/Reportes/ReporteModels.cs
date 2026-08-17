using Viaticos.Domain.Legalizaciones.Enums;

namespace Viaticos.Application.Reportes;

public record ReporteFiltros(
    DateOnly? FechaDesde = null,
    DateOnly? FechaHasta = null,
    Guid? EmpleadoId = null,
    Guid? JefeId = null,
    string? Departamento = null,
    EstadoLegalizacion? Estado = null);

public record ResumenPorEstadoDto(
    string Estado,
    long Cantidad,
    decimal TotalAnticipos,
    decimal TotalGastos,
    decimal TotalReembolsos,
    decimal TotalDevoluciones);

public record LegalizacionDetalleReporteDto(
    Guid Id,
    string Numero,
    string EmpleadoCodigo,
    string EmpleadoNombre,
    string? Departamento,
    string? JefeNombre,
    string Motivo,
    string? Destino,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    string MonedaCodigo,
    string? MonedaSimbolo,
    decimal MontoAnticipo,
    string Estado,
    decimal TotalGastos,
    decimal TotalReembolso,
    decimal TotalDevolucion,
    decimal SaldoAnticipo,
    DateTime CreatedAt,
    DateTime? SubmittedAt,
    DateTime? ClosedAt);

public record GastoPorCategoriaDto(
    string CategoriaCodigo,
    string CategoriaNombre,
    long CantidadGastos,
    decimal TotalMonto,
    decimal PromedioMonto);

public record GastoDetalleReporteDto(
    string LegalizacionNumero,
    string LegalizacionEstado,
    string EmpleadoCodigo,
    string EmpleadoNombre,
    string? Departamento,
    string MonedaCodigo,
    Guid GastoId,
    string CategoriaCodigo,
    string CategoriaNombre,
    DateOnly FechaGasto,
    string Descripcion,
    string? Proveedor,
    string? NumeroDocumento,
    decimal Monto,
    bool Validado,
    long CantidadSoportes);

public record ResumenFinancieroEmpleadoDto(
    Guid EmpleadoId,
    string EmpleadoCodigo,
    string EmpleadoNombre,
    string? Departamento,
    string MonedaCodigo,
    long CantidadLegalizaciones,
    decimal TotalAnticipos,
    decimal TotalGastos,
    decimal TotalReembolsos,
    decimal TotalDevoluciones);

public record PendienteAprobacionReporteDto(
    Guid Id,
    string Numero,
    string EmpleadoCodigo,
    string EmpleadoNombre,
    string? Departamento,
    string Motivo,
    string? Destino,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    string MonedaCodigo,
    decimal MontoAnticipo,
    decimal TotalGastos,
    DateTime? SubmittedAt,
    int? DiasPendientes);

public record PendienteNominaReporteDto(
    Guid Id,
    string Numero,
    string EmpleadoCodigo,
    string EmpleadoNombre,
    string? Departamento,
    string Motivo,
    string MonedaCodigo,
    decimal MontoAnticipo,
    decimal TotalGastos,
    decimal TotalReembolso,
    decimal TotalDevolucion,
    DateTime? SubmittedAt);

public record LegalizacionCerradaReporteDto(
    Guid Id,
    string Numero,
    string EmpleadoCodigo,
    string EmpleadoNombre,
    string? Departamento,
    string MonedaCodigo,
    decimal MontoAnticipo,
    decimal TotalGastos,
    decimal TotalReembolso,
    decimal TotalDevolucion,
    DateTime? ClosedAt);

public record GastoSinSoporteDto(
    string LegalizacionNumero,
    string LegalizacionEstado,
    string EmpleadoNombre,
    string CategoriaNombre,
    DateOnly FechaGasto,
    string Descripcion,
    decimal Monto,
    bool RequiereSoporte);

public record HistorialAuditoriaDto(
    Guid HistorialId,
    string LegalizacionNumero,
    string EmpleadoNombre,
    string? EstadoAnterior,
    string EstadoNuevo,
    string UsuarioNombre,
    string? Comentario,
    DateTime CreatedAt);

public record VolumenMensualDto(
    int Anio,
    int Mes,
    string Periodo,
    long CantidadLegalizaciones,
    decimal TotalAnticipos,
    decimal TotalGastos,
    decimal TotalReembolsos,
    decimal TotalDevoluciones,
    long CantidadCerradas);

public record TiempoPorEstadoDto(
    string LegalizacionNumero,
    string EmpleadoNombre,
    string Estado,
    DateTime InicioEstado,
    DateTime? FinEstado,
    decimal HorasEnEstado);

public enum ReporteTipo
{
    ResumenPorEstado,
    LegalizacionesDetalle,
    GastosPorCategoria,
    GastosDetalle,
    ResumenFinancieroEmpleado,
    PendientesAprobacion,
    PendientesNomina,
    LegalizacionesCerradas,
    GastosSinSoporte,
    HistorialAuditoria,
    VolumenMensual,
    TiemposPorEstado
}
