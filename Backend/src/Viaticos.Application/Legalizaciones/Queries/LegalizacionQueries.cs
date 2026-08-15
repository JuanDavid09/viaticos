using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;

namespace Viaticos.Application.Legalizaciones.Queries;

public record ObtenerLegalizacionQuery(Guid Id) : IRequest<Result<LegalizacionDetalleDto>>;

public class ObtenerLegalizacionQueryHandler : IRequestHandler<ObtenerLegalizacionQuery, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;

    public ObtenerLegalizacionQueryHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _workflow = workflow;
        _currentUser = currentUser;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(ObtenerLegalizacionQuery request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (legalizacion.EmpleadoId != _currentUser.UserId && !_currentUser.IsInRole("Admin"))
        {
            if (_currentUser.IsInRole("JefeAprobador"))
            {
                var jefeAuth = await _workflow.EnsureIsJefeDelEmpleadoAsync(
                    legalizacion,
                    _currentUser.UserId,
                    cancellationToken);
                if (!jefeAuth.IsSuccess)
                    return Result<LegalizacionDetalleDto>.Failure(jefeAuth.ErrorCode!, jefeAuth.Error!);
            }
            else if (!_currentUser.IsInRole("Nomina"))
            {
                return Result<LegalizacionDetalleDto>.Failure("FORBIDDEN", "No tiene permiso para ver esta legalización.");
            }
        }

        var soportes = await _documentoRepository.ListSoportesByGastoIdsAsync(
            legalizacion.Gastos.Select(g => g.Id),
            cancellationToken);

        return Result<LegalizacionDetalleDto>.Success(LegalizacionMapper.ToDetalle(legalizacion, soportes));
    }
}

public record ListarMisLegalizacionesQuery : IRequest<Result<IReadOnlyList<LegalizacionResumenDto>>>;

public class ListarMisLegalizacionesQueryHandler : IRequestHandler<ListarMisLegalizacionesQuery, Result<IReadOnlyList<LegalizacionResumenDto>>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly ICurrentUserService _currentUser;

    public ListarMisLegalizacionesQueryHandler(
        ILegalizacionRepository legalizacionRepository,
        ICurrentUserService currentUser)
    {
        _legalizacionRepository = legalizacionRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<LegalizacionResumenDto>>> Handle(
        ListarMisLegalizacionesQuery request,
        CancellationToken cancellationToken)
    {
        var legalizaciones = await _legalizacionRepository.ListByEmpleadoAsync(_currentUser.UserId, cancellationToken);

        var items = legalizaciones
            .Select(l => LegalizacionMapper.ToResumen(l, l.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<LegalizacionResumenDto>>.Success(items);
    }
}
