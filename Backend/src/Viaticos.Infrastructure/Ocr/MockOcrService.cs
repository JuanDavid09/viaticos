using System.Text.Json;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Documentos;

namespace Viaticos.Infrastructure.Ocr;

/// <summary>
/// OCR simulado para desarrollo local cuando Azure no está configurado.
/// </summary>
public class MockOcrService : IOcrService
{
    public Task<OcrResult> AnalyzeReceiptAsync(Stream document, CancellationToken cancellationToken = default)
    {
        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [OcrFieldNames.Proveedor] = "Proveedor Demo S.A.S.",
            [OcrFieldNames.NumeroDocumento] = $"FAC-{DateTime.UtcNow:yyyyMMdd}-001",
            [OcrFieldNames.Monto] = "125000.00",
            [OcrFieldNames.FechaGasto] = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd")
        };

        var rawJson = JsonSerializer.Serialize(fields);
        return Task.FromResult(new OcrResult(fields, rawJson));
    }
}
