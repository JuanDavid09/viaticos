using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Notificaciones.DTOs;

namespace Viaticos.Application.Notificaciones.Commands;

public record MarcarNotificacionLeidaCommand(Guid NotificacionId) : IRequest<Result<NotificacionDto>>;

public class MarcarNotificacionLeidaCommandHandler
    : IRequestHandler<MarcarNotificacionLeidaCommand, Result<NotificacionDto>>
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public MarcarNotificacionLeidaCommandHandler(
        INotificacionRepository notificacionRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _notificacionRepository = notificacionRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NotificacionDto>> Handle(
        MarcarNotificacionLeidaCommand request,
        CancellationToken cancellationToken)
    {
        var notificacion = await _notificacionRepository.GetByIdForDestinatarioAsync(
            request.NotificacionId,
            _currentUser.UserId,
            cancellationToken);

        if (notificacion is null)
            return Result<NotificacionDto>.Failure("NOT_FOUND", "Notificación no encontrada.");

        notificacion.MarcarLeida();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<NotificacionDto>.Success(NotificacionMapper.ToDto(notificacion));
    }
}

public record MarcarTodasNotificacionesLeidasCommand : IRequest<Result>;

public class MarcarTodasNotificacionesLeidasCommandHandler
    : IRequestHandler<MarcarTodasNotificacionesLeidasCommand, Result>
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly ICurrentUserService _currentUser;

    public MarcarTodasNotificacionesLeidasCommandHandler(
        INotificacionRepository notificacionRepository,
        ICurrentUserService currentUser)
    {
        _notificacionRepository = notificacionRepository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        MarcarTodasNotificacionesLeidasCommand request,
        CancellationToken cancellationToken)
    {
        await _notificacionRepository.MarcarTodasLeidasAsync(_currentUser.UserId, cancellationToken);
        return Result.Success();
    }
}
