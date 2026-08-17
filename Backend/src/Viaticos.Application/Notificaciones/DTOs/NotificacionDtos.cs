using Viaticos.Domain.Core.Entities;

namespace Viaticos.Application.Notificaciones.DTOs;

public record NotificacionDto(
    Guid Id,
    string Tipo,
    string Titulo,
    string Mensaje,
    string? EntidadTipo,
    Guid? EntidadId,
    bool Leida,
    DateTime? LeidaAt,
    DateTime CreatedAt);

public record NotificacionResumenDto(int NoLeidas);

public static class NotificacionMapper
{
    public static NotificacionDto ToDto(Notificacion notificacion) =>
        new(
            notificacion.Id,
            notificacion.Tipo,
            notificacion.Titulo,
            notificacion.Mensaje,
            notificacion.EntidadTipo,
            notificacion.EntidadId,
            notificacion.Leida,
            notificacion.LeidaAt,
            notificacion.CreatedAt);
}
