using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Documentos.DTOs;
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
    short Orden,
    IReadOnlyList<GastoSoporteDto> Soportes);

public record LegalizacionCalendarioDto(
    Guid Id,
    string Numero,
    Guid EmpleadoId,
    string EmpleadoNombre,
    string Motivo,
    string? Destino,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    string Estado,
    decimal MontoAnticipo,
    decimal TotalGastos,
    decimal TotalReembolso,
    decimal TotalDevolucion,
    string MonedaSimbolo,
    DateTime CreatedAt);

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
    IReadOnlyList<GastoDto> Gastos,
    IReadOnlyList<string> AccionesDisponibles);

public record LegalizacionHistorialDto(
    Guid Id,
    string? EstadoAnterior,
    string EstadoNuevo,
    Guid UsuarioId,
    string? Comentario,
    DateTime CreatedAt);

public static class LegalizacionMapper
{
    public static MonedaDto ToDto(Domain.Core.Entities.Moneda moneda) =>
        new(moneda.Id, moneda.CodigoIso.Trim(), moneda.Nombre, moneda.Simbolo);

    public static CategoriaGastoDto ToDto(Domain.Core.Entities.CategoriaGasto categoria) =>
        new(categoria.Id, categoria.Codigo, categoria.Nombre, categoria.RequiereSoporte);

    public static GastoDto ToDto(Gasto gasto, IReadOnlyList<GastoSoporteDto> soportes) =>
        new(
            gasto.Id,
            gasto.CategoriaGastoId,
            gasto.FechaGasto,
            gasto.Descripcion,
            gasto.Proveedor,
            gasto.NumeroDocumento,
            gasto.Monto,
            gasto.Validado,
            gasto.Orden,
            soportes);

    public static LegalizacionCalendarioDto ToCalendario(
        Legalizacion legalizacion,
        string empleadoNombre,
        string monedaSimbolo) =>
        new(
            legalizacion.Id,
            legalizacion.Numero,
            legalizacion.EmpleadoId,
            empleadoNombre,
            legalizacion.Motivo,
            legalizacion.Destino,
            legalizacion.FechaInicio,
            legalizacion.FechaFin,
            legalizacion.Estado.ToString(),
            legalizacion.MontoAnticipo,
            legalizacion.TotalGastos,
            legalizacion.TotalReembolso,
            legalizacion.TotalDevolucion,
            monedaSimbolo,
            legalizacion.CreatedAt);

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

    public static LegalizacionDetalleDto ToDetalle(
        Legalizacion legalizacion,
        IReadOnlyList<GastoSoporteDetalle>? soportes = null,
        IReadOnlyList<string>? accionesDisponibles = null)
    {
        var soportesPorGasto = (soportes ?? [])
            .GroupBy(s => s.GastoId)
            .ToDictionary(g => g.Key, g => g.Select(SoporteMapper.ToDto).ToList());

        return new LegalizacionDetalleDto(
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
            legalizacion.Gastos
                .Select(g => ToDto(g, soportesPorGasto.TryGetValue(g.Id, out var items) ? items : []))
                .ToList(),
            accionesDisponibles ?? []);
    }

    public static LegalizacionHistorialDto ToHistorialDto(LegalizacionHistorial entry) =>
        new(
            entry.Id,
            entry.EstadoAnterior?.ToString(),
            entry.EstadoNuevo.ToString(),
            entry.UsuarioId,
            entry.Comentario,
            entry.CreatedAt);
}
