using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Infrastructure.Identity;
using Viaticos.Infrastructure.Persistence;
using Viaticos.Infrastructure.Persistence.Repositories;

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
        services.AddScoped<DevCurrentUserService>();
        services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<DevCurrentUserService>());

        services.AddScoped<ILegalizacionRepository, LegalizacionRepository>();
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
        services.AddScoped<ICatalogoRepository, CatalogoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
