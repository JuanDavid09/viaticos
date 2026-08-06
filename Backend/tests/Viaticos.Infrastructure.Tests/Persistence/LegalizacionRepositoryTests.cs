using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Legalizaciones.Entities;
using Viaticos.Infrastructure;
using Viaticos.Infrastructure.Persistence;

namespace Viaticos.Infrastructure.Tests.Persistence;

public class LegalizacionRepositoryTests
{
    private static bool RunIntegrationTests =>
        string.Equals(Environment.GetEnvironmentVariable("VIATICOS_INTEGRATION_TESTS"), "1", StringComparison.Ordinal);

    [Fact]
    public async Task GuardarYObtenerLegalizacion_ConPostgreSQL_PersisteGastos()
    {
        if (!RunIntegrationTests)
        {
            return;
        }

        await using var provider = BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();

        var empleadoRepo = scope.ServiceProvider.GetRequiredService<IEmpleadoRepository>();
        var catalogoRepo = scope.ServiceProvider.GetRequiredService<ICatalogoRepository>();
        var legalizacionRepo = scope.ServiceProvider.GetRequiredService<ILegalizacionRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ViaticosDbContext>();

        var empleado = await empleadoRepo.GetByEmailAsync("empleado@empresa.com");
        var moneda = (await catalogoRepo.GetMonedasAsync()).FirstOrDefault(m => m.CodigoIso == "COP");
        var categoria = (await catalogoRepo.GetCategoriasAsync()).FirstOrDefault(c => c.Codigo == "TRANSPORTE");

        Assert.NotNull(empleado);
        Assert.NotNull(moneda);
        Assert.NotNull(categoria);

        var legalizacion = Legalizacion.Crear(
            empleado.Id,
            "Viaje integración",
            new DateOnly(2026, 4, 1),
            new DateOnly(2026, 4, 3),
            moneda.Id,
            100_000,
            empleado.Id);

        legalizacion.AgregarGasto(
            categoria.Id,
            new DateOnly(2026, 4, 2),
            "Taxi",
            25_000,
            empleado.Id);

        await legalizacionRepo.AddAsync(legalizacion);
        await unitOfWork.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        var persisted = await legalizacionRepo.GetByIdAsync(legalizacion.Id);

        Assert.NotNull(persisted);
        Assert.NotEmpty(persisted.Numero);
        Assert.Single(persisted.Gastos);
        Assert.Equal(25_000, persisted.Gastos.First().Monto);
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
        {
            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        "Host=localhost;Port=5432;Database=viaticos;Username=postgres;Password=postgres",
                    ["Jwt:Secret"] = "IntegrationTestSecretKeyMin32Characters!",
                    ["Jwt:Issuer"] = "ViaticosApi",
                    ["Jwt:Audience"] = "ViaticosClient",
                    ["Jwt:ExpirationMinutes"] = "60"
                })
                .Build();
        }

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);
        return services.BuildServiceProvider();
    }
}
