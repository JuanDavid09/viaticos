using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Infrastructure.Identity;
using Viaticos.Infrastructure.Ocr;
using Viaticos.Infrastructure.Persistence;
using Viaticos.Infrastructure.Persistence.Repositories;
using Viaticos.Infrastructure.Storage;

namespace Viaticos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ViaticosDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "public");
            }));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<ILegalizacionRepository, LegalizacionRepository>();
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
        services.AddScoped<ICatalogoRepository, CatalogoRepository>();
        services.AddScoped<IDocumentoRepository, DocumentoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<MinioSettings>(configuration.GetSection(MinioSettings.SectionName));
        services.Configure<AzureOcrSettings>(configuration.GetSection(AzureOcrSettings.SectionName));

        var minioSettings = configuration.GetSection(MinioSettings.SectionName).Get<MinioSettings>() ?? new MinioSettings();
        if (minioSettings.UseLocalFallback)
            services.AddSingleton<IFileStorageService, LocalFileStorageService>();
        else
            services.AddSingleton<IFileStorageService, MinioFileStorageService>();

        var azureOcr = configuration.GetSection(AzureOcrSettings.SectionName).Get<AzureOcrSettings>();
        if (!string.IsNullOrWhiteSpace(azureOcr?.Endpoint) && !string.IsNullOrWhiteSpace(azureOcr.ApiKey))
            services.AddScoped<IOcrService, AzureDocumentIntelligenceOcrService>();
        else
            services.AddScoped<IOcrService, MockOcrService>();

        services.AddJwtAuthentication(configuration);

        return services;
    }
}
