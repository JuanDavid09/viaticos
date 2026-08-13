using System.Globalization;
using Viaticos.Domain.Documentos;

namespace Viaticos.Application.Documentos;

public static class OcrMappingService
{
    public static (string? Proveedor, string? NumeroDocumento, decimal? Monto, DateOnly? FechaGasto) MapToGastoFields(
        IReadOnlyDictionary<string, string> campos)
    {
        campos.TryGetValue(OcrFieldNames.Proveedor, out var proveedor);
        campos.TryGetValue(OcrFieldNames.NumeroDocumento, out var numeroDocumento);

        decimal? monto = null;
        if (campos.TryGetValue(OcrFieldNames.Monto, out var montoRaw) &&
            decimal.TryParse(montoRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out var montoParsed))
        {
            monto = montoParsed;
        }

        DateOnly? fechaGasto = null;
        if (campos.TryGetValue(OcrFieldNames.FechaGasto, out var fechaRaw) &&
            DateOnly.TryParse(fechaRaw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaParsed))
        {
            fechaGasto = fechaParsed;
        }

        return (proveedor, numeroDocumento, monto, fechaGasto);
    }
}
