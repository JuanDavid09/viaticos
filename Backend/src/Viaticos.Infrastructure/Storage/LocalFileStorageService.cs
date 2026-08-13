using Microsoft.Extensions.Options;
using Viaticos.Application.Common.Interfaces;

namespace Viaticos.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly MinioSettings _settings;

    public LocalFileStorageService(IOptions<MinioSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<(string bucket, string objectKey)> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var bucket = _settings.Bucket;
        var objectKey = $"{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid()}-{SanitizeFileName(fileName)}";
        var fullPath = Path.Combine(_settings.LocalPath, bucket, objectKey.Replace('/', Path.DirectorySeparatorChar));

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, cancellationToken);

        return (bucket, objectKey);
    }

    public Task<Stream> DownloadAsync(string bucket, string objectKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_settings.LocalPath, bucket, objectKey.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Archivo no encontrado: {objectKey}");

        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult(stream);
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName);
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');

        return string.IsNullOrWhiteSpace(name) ? "archivo" : name;
    }
}
