using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Viaticos.Application.Common.Behaviors;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Legalizaciones.Services;

namespace Viaticos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<ILegalizacionWorkflowService, LegalizacionWorkflowService>();

        return services;
    }
}
