using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Empleados;

namespace Viaticos.Application.Legalizaciones.Queries;

public record ListarEmpleadosAsignablesQuery : IRequest<Result<IReadOnlyList<EmpleadoDto>>>;

public class ListarEmpleadosAsignablesQueryHandler
    : IRequestHandler<ListarEmpleadosAsignablesQuery, Result<IReadOnlyList<EmpleadoDto>>>
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly ICurrentUserService _currentUser;

    public ListarEmpleadosAsignablesQueryHandler(
        IEmpleadoRepository empleadoRepository,
        ICurrentUserService currentUser)
    {
        _empleadoRepository = empleadoRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<EmpleadoDto>>> Handle(
        ListarEmpleadosAsignablesQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole("Admin") && !_currentUser.IsInRole("JefeAprobador"))
        {
            return Result<IReadOnlyList<EmpleadoDto>>.Failure(
                "FORBIDDEN",
                "Solo administradores y jefes pueden asignar legalizaciones.");
        }

        var jefeFilter = _currentUser.IsInRole("Admin") ? (Guid?)null : _currentUser.UserId;
        var empleados = await _empleadoRepository.ListAsignablesLegalizacionAsync(jefeFilter, cancellationToken);

        var items = empleados
            .Select(EmpleadoMapper.ToDto)
            .ToList();

        return Result<IReadOnlyList<EmpleadoDto>>.Success(items);
    }
}
