using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Notificaciones.DTOs;

namespace Viaticos.Application.Notificaciones.Queries;

public record ListarNotificacionesQuery(int Limite = 20) : IRequest<Result<IReadOnlyList<NotificacionDto>>>;

public class ListarNotificacionesQueryHandler
    : IRequestHandler<ListarNotificacionesQuery, Result<IReadOnlyList<NotificacionDto>>>
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly ICurrentUserService _currentUser;

    public ListarNotificacionesQueryHandler(
        INotificacionRepository notificacionRepository,
        ICurrentUserService currentUser)
    {
        _notificacionRepository = notificacionRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<NotificacionDto>>> Handle(
        ListarNotificacionesQuery request,
        CancellationToken cancellationToken)
    {
        var limite = Math.Clamp(request.Limite, 1, 50);
        var notificaciones = await _notificacionRepository.ListByDestinatarioAsync(
            _currentUser.UserId,
            limite,
            cancellationToken);

        var items = notificaciones
            .Select(NotificacionMapper.ToDto)
            .ToList();

        return Result<IReadOnlyList<NotificacionDto>>.Success(items);
    }
}

public record ObtenerResumenNotificacionesQuery : IRequest<Result<NotificacionResumenDto>>;

public class ObtenerResumenNotificacionesQueryHandler
    : IRequestHandler<ObtenerResumenNotificacionesQuery, Result<NotificacionResumenDto>>
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly ICurrentUserService _currentUser;

    public ObtenerResumenNotificacionesQueryHandler(
        INotificacionRepository notificacionRepository,
        ICurrentUserService currentUser)
    {
        _notificacionRepository = notificacionRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<NotificacionResumenDto>> Handle(
        ObtenerResumenNotificacionesQuery request,
        CancellationToken cancellationToken)
    {
        var noLeidas = await _notificacionRepository.CountNoLeidasAsync(_currentUser.UserId, cancellationToken);
        return Result<NotificacionResumenDto>.Success(new NotificacionResumenDto(noLeidas));
    }
}
