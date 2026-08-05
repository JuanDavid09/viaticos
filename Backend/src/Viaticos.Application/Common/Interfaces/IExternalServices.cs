namespace Viaticos.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<(string bucket, string objectKey)> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(string bucket, string objectKey, CancellationToken cancellationToken = default);
}

public interface IOcrService
{
    Task<OcrResult> AnalyzeReceiptAsync(Stream document, CancellationToken cancellationToken = default);
}

public record OcrResult(
    IReadOnlyDictionary<string, string> Fields,
    string RawJson);
