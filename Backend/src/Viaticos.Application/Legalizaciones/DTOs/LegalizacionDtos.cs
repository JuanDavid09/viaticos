using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Legalizaciones.DTOs;

public record MonedaDto(Guid Id, string CodigoIso, string Nombre, string? Simbolo);

public record CategoriaGastoDto(Guid Id, string Codigo, string Nombre, bool RequiereSoporte);

public record CatalogosDto(IReadOnlyList<MonedaDto> Monedas, IReadOnlyList<CategoriaGastoDto> Categorias);

public record GastoDto(
    Guid Id,
    Guid CategoriaGastoId,
    DateOnly FechaGasto,
    string Descripcion,
    string? Proveedor,
    string? NumeroDocumento,
    decimal Monto,
    bool Validado,
    short Orden);

public record LegalizacionResumenDto(
    Guid Id,
    string Numero,
    string Motivo,
    string? Destino,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    string Estado,
    decimal TotalGastos,
    decimal TotalReembolso,
    decimal TotalDevolucion,
    DateTime CreatedAt);

public record LegalizacionDetalleDto(
    Guid Id,
    string Numero,
    Guid EmpleadoId,
    string Motivo,
    string? Destino,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    Guid MonedaId,
    decimal MontoAnticipo,
    string Estado,
    decimal TotalGastos,
    decimal TotalReembolso,
    decimal TotalDevolucion,
    string? Observaciones,
    IReadOnlyList<GastoDto> Gastos);

public static class LegalizacionMapper
{
    public static MonedaDto ToDto(Domain.Core.Entities.Moneda moneda) =>
        new(moneda.Id, moneda.CodigoIso.Trim(), moneda.Nombre, moneda.Simbolo);

    public static CategoriaGastoDto ToDto(Domain.Core.Entities.CategoriaGasto categoria) =>
        new(categoria.Id, categoria.Codigo, categoria.Nombre, categoria.RequiereSoporte);

    public static GastoDto ToDto(Gasto gasto) =>
        new(
            gasto.Id,
            gasto.CategoriaGastoId,
            gasto.FechaGasto,
            gasto.Descripcion,
            gasto.Proveedor,
            gasto.NumeroDocumento,
            gasto.Monto,
            gasto.Validado,
            gasto.Orden);

    public static LegalizacionResumenDto ToResumen(Legalizacion legalizacion, DateTime createdAt) =>
        new(
            legalizacion.Id,
            legalizacion.Numero,
            legalizacion.Motivo,
            legalizacion.Destino,
            legalizacion.FechaInicio,
            legalizacion.FechaFin,
            legalizacion.Estado.ToString(),
            legalizacion.TotalGastos,
            legalizacion.TotalReembolso,
            legalizacion.TotalDevolucion,
            createdAt);

    public static LegalizacionDetalleDto ToDetalle(Legalizacion legalizacion) =>
        new(
            legalizacion.Id,
            legalizacion.Numero,
            legalizacion.EmpleadoId,
            legalizacion.Motivo,
            legalizacion.Destino,
            legalizacion.FechaInicio,
            legalizacion.FechaFin,
            legalizacion.MonedaId,
            legalizacion.MontoAnticipo,
            legalizacion.Estado.ToString(),
            legalizacion.TotalGastos,
            legalizacion.TotalReembolso,
            legalizacion.TotalDevolucion,
            legalizacion.Observaciones,
            legalizacion.Gastos.Select(ToDto).ToList());
}
