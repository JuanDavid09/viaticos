using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;

namespace Viaticos.Application.Legalizaciones.Queries;

public record ListarPendientesAprobacionQuery : IRequest<Result<IReadOnlyList<LegalizacionResumenDto>>>;

public class ListarPendientesAprobacionQueryHandler
    : IRequestHandler<ListarPendientesAprobacionQuery, Result<IReadOnlyList<LegalizacionResumenDto>>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly ICurrentUserService _currentUser;

    public ListarPendientesAprobacionQueryHandler(
        ILegalizacionRepository legalizacionRepository,
        ICurrentUserService currentUser)
    {
        _legalizacionRepository = legalizacionRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<LegalizacionResumenDto>>> Handle(
        ListarPendientesAprobacionQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole("JefeAprobador") && !_currentUser.IsInRole("Admin"))
            return Result<IReadOnlyList<LegalizacionResumenDto>>.Failure("FORBIDDEN", "Solo jefes pueden ver esta bandeja.");

        var legalizaciones = await _legalizacionRepository.ListPendientesAprobacionByJefeAsync(
            _currentUser.UserId,
            cancellationToken);

        var items = legalizaciones
            .Select(l => LegalizacionMapper.ToResumen(l, l.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<LegalizacionResumenDto>>.Success(items);
    }
}

public record ListarPendientesNominaQuery : IRequest<Result<IReadOnlyList<LegalizacionResumenDto>>>;

public class ListarPendientesNominaQueryHandler
    : IRequestHandler<ListarPendientesNominaQuery, Result<IReadOnlyList<LegalizacionResumenDto>>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly ICurrentUserService _currentUser;

    public ListarPendientesNominaQueryHandler(
        ILegalizacionRepository legalizacionRepository,
        ICurrentUserService currentUser)
    {
        _legalizacionRepository = legalizacionRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<LegalizacionResumenDto>>> Handle(
        ListarPendientesNominaQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole("Nomina") && !_currentUser.IsInRole("Admin"))
            return Result<IReadOnlyList<LegalizacionResumenDto>>.Failure("FORBIDDEN", "Solo nómina puede ver esta bandeja.");

        var legalizaciones = await _legalizacionRepository.ListPendientesNominaAsync(cancellationToken);

        var items = legalizaciones
            .Select(l => LegalizacionMapper.ToResumen(l, l.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<LegalizacionResumenDto>>.Success(items);
    }
}

public record ObtenerHistorialQuery(Guid LegalizacionId) : IRequest<Result<IReadOnlyList<LegalizacionHistorialDto>>>;

public class ObtenerHistorialQueryHandler : IRequestHandler<ObtenerHistorialQuery, Result<IReadOnlyList<LegalizacionHistorialDto>>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly ICurrentUserService _currentUser;

    public ObtenerHistorialQueryHandler(
        ILegalizacionRepository legalizacionRepository,
        ICurrentUserService currentUser)
    {
        _legalizacionRepository = legalizacionRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<LegalizacionHistorialDto>>> Handle(
        ObtenerHistorialQuery request,
        CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.LegalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<IReadOnlyList<LegalizacionHistorialDto>>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (legalizacion.EmpleadoId != _currentUser.UserId
            && !_currentUser.IsInRole("Admin")
            && !_currentUser.IsInRole("JefeAprobador")
            && !_currentUser.IsInRole("Nomina"))
        {
            return Result<IReadOnlyList<LegalizacionHistorialDto>>.Failure("FORBIDDEN", "No tiene permiso para ver el historial.");
        }

        var historial = await _legalizacionRepository.GetHistorialAsync(request.LegalizacionId, cancellationToken);
        var items = historial
            .Select(LegalizacionMapper.ToHistorialDto)
            .ToList();

        return Result<IReadOnlyList<LegalizacionHistorialDto>>.Success(items);
    }
}
