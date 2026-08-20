using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;
using Viaticos.Application.Legalizaciones.Services;

namespace Viaticos.Application.Legalizaciones.Queries;

public record ObtenerLegalizacionQuery(Guid Id) : IRequest<Result<LegalizacionDetalleDto>>;

public class ObtenerLegalizacionQueryHandler : IRequestHandler<ObtenerLegalizacionQuery, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly ICurrentUserService _currentUser;

    public ObtenerLegalizacionQueryHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionWorkflowService workflow,
        ILegalizacionDetalleFactory detalleFactory,
        ICurrentUserService currentUser)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _workflow = workflow;
        _detalleFactory = detalleFactory;
        _currentUser = currentUser;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(ObtenerLegalizacionQuery request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        var viewAuth = await _workflow.CanViewLegalizacionAsync(legalizacion, _currentUser, cancellationToken);
        if (!viewAuth.IsSuccess)
            return Result<LegalizacionDetalleDto>.Failure(viewAuth.ErrorCode!, viewAuth.Error!);

        var soportes = await _documentoRepository.ListSoportesByGastoIdsAsync(
            legalizacion.Gastos.Select(g => g.Id),
            cancellationToken);

        return Result<LegalizacionDetalleDto>.Success(
            await _detalleFactory.CreateAsync(legalizacion, soportes, cancellationToken));
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
