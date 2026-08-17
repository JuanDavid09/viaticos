using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;

namespace Viaticos.Application.Legalizaciones.Queries;

public record ListarCalendarioLegalizacionesQuery(DateOnly? Desde, DateOnly? Hasta)
    : IRequest<Result<IReadOnlyList<LegalizacionCalendarioDto>>>;

public class ListarCalendarioLegalizacionesQueryHandler
    : IRequestHandler<ListarCalendarioLegalizacionesQuery, Result<IReadOnlyList<LegalizacionCalendarioDto>>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly ICurrentUserService _currentUser;

    public ListarCalendarioLegalizacionesQueryHandler(
        ILegalizacionRepository legalizacionRepository,
        ICurrentUserService currentUser)
    {
        _legalizacionRepository = legalizacionRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<LegalizacionCalendarioDto>>> Handle(
        ListarCalendarioLegalizacionesQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole("Admin") && !_currentUser.IsInRole("JefeAprobador"))
        {
            return Result<IReadOnlyList<LegalizacionCalendarioDto>>.Failure(
                "FORBIDDEN",
                "Solo administradores y jefes pueden ver el calendario del equipo.");
        }

        var jefeFilter = _currentUser.IsInRole("Admin") ? (Guid?)null : _currentUser.UserId;

        var entries = await _legalizacionRepository.ListCalendarioAsync(
            jefeFilter,
            request.Desde,
            request.Hasta,
            cancellationToken);

        var items = entries
            .Select(entry => LegalizacionMapper.ToCalendario(
                entry.Legalizacion,
                entry.EmpleadoNombre,
                entry.MonedaSimbolo))
            .ToList();

        return Result<IReadOnlyList<LegalizacionCalendarioDto>>.Success(items);
    }
}
