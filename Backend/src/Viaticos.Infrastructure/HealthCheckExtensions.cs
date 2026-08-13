using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viaticos.Infrastructure.Health;

namespace Viaticos.Infrastructure;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddViaticosHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "postgresql", tags: ["db", "ready"])
            .AddCheck<MinioHealthCheck>("minio", tags: ["storage", "ready"]);

        return services;
    }
}
