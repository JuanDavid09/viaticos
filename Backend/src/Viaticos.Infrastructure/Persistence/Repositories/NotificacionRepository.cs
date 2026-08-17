using Microsoft.EntityFrameworkCore;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Core.Entities;
using Viaticos.Domain.Core.Enums;

namespace Viaticos.Infrastructure.Persistence.Repositories;

internal class NotificacionRepository : INotificacionRepository
{
    private readonly ViaticosDbContext _context;

    public NotificacionRepository(ViaticosDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Notificacion>> ListByDestinatarioAsync(
        Guid destinatarioId,
        int limite,
        CancellationToken cancellationToken = default)
    {
        return await _context.Notificaciones
            .Where(n => n.DestinatarioId == destinatarioId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountNoLeidasAsync(Guid destinatarioId, CancellationToken cancellationToken = default)
    {
        return await _context.Notificaciones
            .CountAsync(n => n.DestinatarioId == destinatarioId && !n.Leida, cancellationToken);
    }

    public async Task<Notificacion?> GetByIdForDestinatarioAsync(
        Guid id,
        Guid destinatarioId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Notificaciones
            .FirstOrDefaultAsync(n => n.Id == id && n.DestinatarioId == destinatarioId, cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<Notificacion> notificaciones,
        CancellationToken cancellationToken = default)
    {
        await _context.Notificaciones.AddRangeAsync(notificaciones, cancellationToken);
    }

    public async Task MarcarTodasLeidasAsync(Guid destinatarioId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        await _context.Notificaciones
            .Where(n => n.DestinatarioId == destinatarioId && !n.Leida)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(n => n.Leida, true)
                    .SetProperty(n => n.LeidaAt, now),
                cancellationToken);
    }
}
