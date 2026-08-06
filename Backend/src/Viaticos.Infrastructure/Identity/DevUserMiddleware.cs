using Microsoft.AspNetCore.Http;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Infrastructure.Identity;

namespace Viaticos.Infrastructure.Identity;

public class DevUserMiddleware
{
    private const string DefaultEmail = "empleado@empresa.com";
    private readonly RequestDelegate _next;

    public DevUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IEmpleadoRepository empleadoRepository,
        DevCurrentUserService currentUserService)
    {
        if (context.Request.Path.StartsWithSegments("/api/health"))
        {
            await _next(context);
            return;
        }

        var email = context.Request.Headers["X-Dev-User-Email"].FirstOrDefault() ?? DefaultEmail;
        var empleado = await empleadoRepository.GetByEmailAsync(email);

        if (empleado is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Usuario no encontrado",
                detail = $"No existe empleado con email '{email}'. Use un email del seed o header X-Dev-User-Email."
            });
            return;
        }

        currentUserService.CurrentEmpleado = empleado;
        await _next(context);
    }
}
