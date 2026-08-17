using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Viaticos.Infrastructure.Persistence;

public static class DatabaseMigrationBootstrap
{
    private const string NotificacionesSql = """
        CREATE TABLE IF NOT EXISTS core.notificacion (
            id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
            destinatario_id UUID NOT NULL REFERENCES core.empleado(id),
            tipo            VARCHAR(50) NOT NULL,
            titulo          VARCHAR(200) NOT NULL,
            mensaje         TEXT NOT NULL,
            entidad_tipo    VARCHAR(50),
            entidad_id      UUID,
            leida           BOOLEAN NOT NULL DEFAULT FALSE,
            leida_at        TIMESTAMPTZ,
            created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
        );

        CREATE INDEX IF NOT EXISTS idx_notificacion_destinatario_leida
            ON core.notificacion (destinatario_id, leida, created_at DESC);
        """;

    public static async Task ApplyPendingMigrationsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ViaticosDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ViaticosDbContext>>();

        if (!await NotificacionesTableExistsAsync(context))
        {
            logger.LogWarning("Falta la tabla core.notificacion. Aplicando script 010_notificaciones.sql…");
            await context.Database.ExecuteSqlRawAsync(NotificacionesSql);
            logger.LogInformation("Tabla core.notificacion creada correctamente.");
        }
        else
        {
            logger.LogDebug("Esquema de notificaciones verificado.");
        }

        if (!await ReportesSchemaExistsAsync(context))
        {
            var reportesSqlPath = ResolveReportesSqlPath();
            if (reportesSqlPath is null)
            {
                logger.LogWarning(
                    "Falta el schema reportes y no se encontró database/011_reportes.sql. Ejecútelo manualmente.");
                return;
            }

            logger.LogWarning("Falta el schema reportes. Aplicando {Path}…", reportesSqlPath);
            var sql = await File.ReadAllTextAsync(reportesSqlPath);
            await context.Database.ExecuteSqlRawAsync(sql);
            logger.LogInformation("Procedimientos de reportes instalados correctamente.");
        }
        else
        {
            logger.LogDebug("Schema reportes verificado.");
        }
    }

    private static string? ResolveReportesSqlPath()
    {
        var candidates = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "database", "011_reportes.sql"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "database", "011_reportes.sql"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "database", "011_reportes.sql"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database", "011_reportes.sql"))
        };

        return candidates.Select(Path.GetFullPath).FirstOrDefault(File.Exists);
    }

    private static async Task<bool> NotificacionesTableExistsAsync(ViaticosDbContext context)
    {
        await context.Database.OpenConnectionAsync();
        try
        {
            await using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = """
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.tables
                    WHERE table_schema = 'core'
                      AND table_name = 'notificacion'
                );
                """;

            var result = await command.ExecuteScalarAsync();
            return result is bool exists && exists;
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }
    }

    private static async Task<bool> ReportesSchemaExistsAsync(ViaticosDbContext context)
    {
        await context.Database.OpenConnectionAsync();
        try
        {
            await using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = """
                SELECT EXISTS (
                    SELECT 1
                    FROM pg_proc p
                    JOIN pg_namespace n ON n.oid = p.pronamespace
                    WHERE n.nspname = 'reportes'
                      AND p.proname = 'sp_resumen_por_estado'
                );
                """;

            var result = await command.ExecuteScalarAsync();
            return result is bool exists && exists;
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }
    }
}
