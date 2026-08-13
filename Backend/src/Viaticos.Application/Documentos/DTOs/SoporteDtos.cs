using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Documentos.Entities;

namespace Viaticos.Application.Documentos.DTOs;

public record OcrCampoDto(
    Guid Id,
    string NombreCampo,
    string? ValorExtraido,
    string? ValorValidado,
    bool Validado);

public record OcrExtraccionDto(
    Guid Id,
    Guid GastoSoporteId,
    string Estado,
    string? ErrorMensaje,
    DateTime? ProcesadoAt,
    IReadOnlyList<OcrCampoDto> Campos);

public record GastoSoporteDto(
    Guid Id,
    Guid ArchivoId,
    string NombreOriginal,
    string MimeType,
    long TamanoBytes,
    bool EsPrincipal,
    Guid? OcrExtraccionId,
    string? OcrEstado);

public record SubirSoporteResponseDto(
    Guid GastoSoporteId,
    Guid ArchivoId,
    Guid OcrExtraccionId,
    string NombreOriginal);

public static class SoporteMapper
{
    public static GastoSoporteDto ToDto(GastoSoporteDetalle detalle) =>
        new(
            detalle.Id,
            detalle.ArchivoId,
            detalle.NombreOriginal,
            detalle.MimeType,
            detalle.TamanoBytes,
            detalle.EsPrincipal,
            detalle.OcrExtraccionId,
            detalle.OcrEstado);

    public static OcrExtraccionDto ToDto(OcrExtraccion extraccion) =>
        new(
            extraccion.Id,
            extraccion.GastoSoporteId!.Value,
            extraccion.Estado.ToString(),
            extraccion.ErrorMensaje,
            extraccion.ProcesadoAt,
            extraccion.Campos.Select(c => new OcrCampoDto(
                c.Id,
                c.NombreCampo,
                c.ValorExtraido,
                c.ValorValidado,
                c.Validado)).ToList());
}
