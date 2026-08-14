using System.Text.Json;
using Viaticos.Infrastructure.Identity;

namespace Viaticos.Api.Middleware;

public class MustChangePasswordMiddleware
{
    private static readonly HashSet<string> AllowedPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/auth/change-password",
    };

    private readonly RequestDelegate _next;

    public MustChangePasswordMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var mustChange = context.User.FindFirst(JwtTokenService.MustChangePasswordClaim)?.Value == "true";
            var path = context.Request.Path.Value ?? string.Empty;

            if (mustChange && !IsAllowedPath(path))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    code = "MUST_CHANGE_PASSWORD",
                    message = "Debe cambiar su contraseña antes de continuar.",
                }));
                return;
            }
        }

        await _next(context);
    }

    private static bool IsAllowedPath(string path) =>
        AllowedPaths.Contains(path);
}
