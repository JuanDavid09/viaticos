namespace Viaticos.Infrastructure.Storage;

public class MinioSettings
{
    public const string SectionName = "Minio";

    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = "minioadmin";
    public string SecretKey { get; set; } = "minioadmin";
    public string Bucket { get; set; } = "viaticos";
    public bool UseSsl { get; set; }
    public bool UseLocalFallback { get; set; } = true;
    public string LocalPath { get; set; } = "uploads";
}
