using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Viaticos.Infrastructure.Storage;

namespace Viaticos.Infrastructure.Health;

public class MinioHealthCheck : IHealthCheck
{
    private readonly MinioSettings _settings;

    public MinioHealthCheck(IOptions<MinioSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_settings.UseLocalFallback)
        {
            return HealthCheckResult.Healthy("Almacenamiento local (MinIO fallback desactivado).");
        }

        try
        {
            var client = new MinioClient()
                .WithEndpoint(_settings.Endpoint)
                .WithCredentials(_settings.AccessKey, _settings.SecretKey)
                .WithSSL(_settings.UseSsl)
                .Build();

            var exists = await client.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_settings.Bucket),
                cancellationToken);

            if (!exists)
            {
                await client.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(_settings.Bucket),
                    cancellationToken);
            }

            return HealthCheckResult.Healthy($"MinIO disponible (bucket '{_settings.Bucket}').");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MinIO no disponible.", ex);
        }
    }
}
