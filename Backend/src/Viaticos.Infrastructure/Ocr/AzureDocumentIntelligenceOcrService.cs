using System.Globalization;
using System.Text.Json;
using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.Extensions.Options;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Documentos;

namespace Viaticos.Infrastructure.Ocr;

public class AzureDocumentIntelligenceOcrService : IOcrService
{
    private readonly DocumentIntelligenceClient _client;

    public AzureDocumentIntelligenceOcrService(IOptions<AzureOcrSettings> settings)
    {
        var config = settings.Value;
        _client = new DocumentIntelligenceClient(new Uri(config.Endpoint), new AzureKeyCredential(config.ApiKey));
    }

    public async Task<OcrResult> AnalyzeReceiptAsync(Stream document, CancellationToken cancellationToken = default)
    {
        var content = new BinaryData(await ReadAllBytesAsync(document, cancellationToken));

        var operation = await _client.AnalyzeDocumentAsync(
            WaitUntil.Completed,
            "prebuilt-receipt",
            content,
            cancellationToken: cancellationToken);

        var result = operation.Value;
        var fields = MapReceiptFields(result);
        var rawJson = JsonSerializer.Serialize(result);

        return new OcrResult(fields, rawJson);
    }

    private static IReadOnlyDictionary<string, string> MapReceiptFields(AnalyzeResult result)
    {
        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var document = result.Documents.FirstOrDefault();
        if (document?.Fields is null)
            return fields;

        if (TryGetField(document.Fields, "MerchantName", out var merchant))
            fields[OcrFieldNames.Proveedor] = merchant;

        if (TryGetField(document.Fields, "InvoiceId", out var invoiceId))
            fields[OcrFieldNames.NumeroDocumento] = invoiceId;

        if (TryGetField(document.Fields, "Total", out var total))
            fields[OcrFieldNames.Monto] = total;

        if (TryGetField(document.Fields, "TransactionDate", out var date))
            fields[OcrFieldNames.FechaGasto] = date;

        return fields;
    }

    private static bool TryGetField(IReadOnlyDictionary<string, DocumentField> fields, string key, out string value)
    {
        value = string.Empty;
        if (!fields.TryGetValue(key, out var field) || field?.Content is null)
            return false;

        if (field.FieldType == DocumentFieldType.Date && field.ValueDate.HasValue)
        {
            value = field.ValueDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return true;
        }

        if (field.FieldType == DocumentFieldType.Double && field.ValueDouble.HasValue)
        {
            value = field.ValueDouble.Value.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        if (field.FieldType == DocumentFieldType.Currency && field.ValueCurrency is not null)
        {
            value = field.ValueCurrency.Amount.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        value = field.Content;
        return !string.IsNullOrWhiteSpace(value);
    }

    private static async Task<byte[]> ReadAllBytesAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (stream is MemoryStream memoryStream)
            return memoryStream.ToArray();

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        return ms.ToArray();
    }
}
