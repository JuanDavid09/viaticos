using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viaticos.Application;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Legalizaciones.Commands;
using Viaticos.Domain.Legalizaciones.Entities;
using Viaticos.Infrastructure;

namespace Viaticos.Infrastructure.Tests.Persistence;

public class AgregarGastoCommandTests
{
    private static bool RunIntegrationTests =>
        string.Equals(Environment.GetEnvironmentVariable("VIATICOS_INTEGRATION_TESTS"), "1", StringComparison.Ordinal);

    [Fact]
    public async Task AgregarGasto_EnLegalizacionExistente_PersisteYNotifica()
    {
        if (!RunIntegrationTests)
            return;

        await using var provider = BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();

        var empleadoRepo = scope.ServiceProvider.GetRequiredService<IEmpleadoRepository>();
        var catalogoRepo = scope.ServiceProvider.GetRequiredService<ICatalogoRepository>();
        var legalizacionRepo = scope.ServiceProvider.GetRequiredService<ILegalizacionRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var empleado = await empleadoRepo.GetByEmailAsync("empleado@empresa.com");
        var moneda = (await catalogoRepo.GetMonedasAsync()).FirstOrDefault(m => m.CodigoIso == "COP");
        var categoria = (await catalogoRepo.GetCategoriasAsync()).FirstOrDefault(c => c.Codigo == "TRANSPORTE");

        Assert.NotNull(empleado);
        Assert.NotNull(moneda);
        Assert.NotNull(categoria);

        var legalizacion = Legalizacion.Crear(
            empleado.Id,
            "Viaje agregar gasto",
            new DateOnly(2026, 4, 10),
            new DateOnly(2026, 4, 12),
            moneda.Id,
            100_000,
            empleado.Id);

        await legalizacionRepo.AddAsync(legalizacion);
        await unitOfWork.SaveChangesAsync();

        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUserService>() as TestCurrentUserService;
        Assert.NotNull(currentUser);
        currentUser.SetUser(empleado.Id, "Empleado");

        var result = await mediator.Send(
            new AgregarGastoCommand(
                legalizacion.Id,
                categoria.Id,
                new DateOnly(2026, 4, 11),
                "Taxi aeropuerto",
                35_000,
                "Transportes SA",
                "FAC-001"));

        Assert.True(result.IsSuccess, result.Error ?? "AgregarGasto falló");
        Assert.Single(result.Value!.Gastos);
        Assert.Equal(35_000, result.Value.Gastos.First().Monto);
        Assert.Equal(35_000, result.Value.TotalGastos);
    }

    [Fact]
    public async Task EnviarValidacion_ConGastos_CambiaEstado()
    {
        if (!RunIntegrationTests)
            return;

        await using var provider = BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();

        var empleadoRepo = scope.ServiceProvider.GetRequiredService<IEmpleadoRepository>();
        var catalogoRepo = scope.ServiceProvider.GetRequiredService<ICatalogoRepository>();
        var legalizacionRepo = scope.ServiceProvider.GetRequiredService<ILegalizacionRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var empleado = await empleadoRepo.GetByEmailAsync("empleado@empresa.com");
        var moneda = (await catalogoRepo.GetMonedasAsync()).FirstOrDefault(m => m.CodigoIso == "COP");
        var categoria = (await catalogoRepo.GetCategoriasAsync()).FirstOrDefault(c => c.Codigo == "TRANSPORTE");

        Assert.NotNull(empleado);
        Assert.NotNull(moneda);
        Assert.NotNull(categoria);

        var legalizacion = Legalizacion.Crear(
            empleado.Id,
            "Viaje enviar validacion",
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 3),
            moneda.Id,
            50_000,
            empleado.Id);

        legalizacion.AgregarGasto(
            categoria.Id,
            new DateOnly(2026, 5, 2),
            "Hotel",
            120_000,
            empleado.Id);

        await legalizacionRepo.AddAsync(legalizacion);
        await unitOfWork.SaveChangesAsync();

        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUserService>() as TestCurrentUserService;
        Assert.NotNull(currentUser);
        currentUser.SetUser(empleado.Id, "Empleado");

        var result = await mediator.Send(new EnviarValidacionCommand(legalizacion.Id));

        Assert.True(result.IsSuccess, result.Error ?? "EnviarValidacion falló");
        Assert.Equal("PendienteValidacion", result.Value!.Estado);
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile(Path.Combine("..", "..", "..", "..", "src", "Viaticos.Api", "appsettings.json"), optional: true)
            .Build();

        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
        {
            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        "Host=localhost;Port=5432;Database=viaticos;Username=postgres;Password=AdminBd",
                    ["Jwt:Secret"] = "IntegrationTestSecretKeyMin32Characters!",
                    ["Jwt:Issuer"] = "ViaticosApi",
                    ["Jwt:Audience"] = "ViaticosClient",
                    ["Jwt:ExpirationMinutes"] = "60"
                })
                .Build();
        }

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddInfrastructure(configuration);
        services.AddApplication();
        services.AddScoped<ICurrentUserService, TestCurrentUserService>();
        return services.BuildServiceProvider();
    }
}

internal sealed class TestCurrentUserService : ICurrentUserService
{
    public Guid UserId { get; private set; }
    public string Email { get; private set; } = "empleado@empresa.com";
    public string Rol { get; private set; } = "Empleado";

    public void SetUser(Guid userId, string rol, string? email = null)
    {
        UserId = userId;
        Rol = rol;
        if (!string.IsNullOrWhiteSpace(email))
            Email = email;
    }

    public bool IsInRole(string rol) =>
        string.Equals(Rol, rol, StringComparison.OrdinalIgnoreCase);
}
