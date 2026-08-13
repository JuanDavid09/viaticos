using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Viaticos.Application.Common.Interfaces;

namespace Viaticos.Infrastructure.Storage;

public class MinioFileStorageService : IFileStorageService
{
    private readonly MinioSettings _settings;
    private readonly ILogger<MinioFileStorageService> _logger;
    private readonly IMinioClient _client;

    public MinioFileStorageService(IOptions<MinioSettings> settings, ILogger<MinioFileStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        _client = new MinioClient()
            .WithEndpoint(_settings.Endpoint)
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .WithSSL(_settings.UseSsl)
            .Build();
    }

    public async Task<(string bucket, string objectKey)> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketAsync(cancellationToken);

        var objectKey = $"{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid()}-{Path.GetFileName(fileName)}";

        var putArgs = new PutObjectArgs()
            .WithBucket(_settings.Bucket)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType);

        await _client.PutObjectAsync(putArgs, cancellationToken);

        return (_settings.Bucket, objectKey);
    }

    public async Task<Stream> DownloadAsync(string bucket, string objectKey, CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        var getArgs = new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _client.GetObjectAsync(getArgs, cancellationToken);
        memoryStream.Position = 0;
        return memoryStream;
    }

    private async Task EnsureBucketAsync(CancellationToken cancellationToken)
    {
        var existsArgs = new BucketExistsArgs().WithBucket(_settings.Bucket);
        var exists = await _client.BucketExistsAsync(existsArgs, cancellationToken);

        if (exists)
            return;

        var makeArgs = new MakeBucketArgs().WithBucket(_settings.Bucket);
        await _client.MakeBucketAsync(makeArgs, cancellationToken);
        _logger.LogInformation("Bucket MinIO creado: {Bucket}", _settings.Bucket);
    }
}
