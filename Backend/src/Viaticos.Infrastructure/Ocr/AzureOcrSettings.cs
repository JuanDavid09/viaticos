namespace Viaticos.Infrastructure.Ocr;

public class AzureOcrSettings
{
    public const string SectionName = "AzureOcr";

    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
