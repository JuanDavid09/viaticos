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
            return;
        }

        logger.LogDebug("Esquema de notificaciones verificado.");
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
}
