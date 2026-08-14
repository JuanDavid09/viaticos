using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;

namespace Viaticos.Application.Empleados.Queries;

public record ListarEmpleadosQuery(bool IncludeInactive = false)
    : IRequest<Result<IReadOnlyList<EmpleadoDto>>>;

public class ListarEmpleadosQueryHandler
    : IRequestHandler<ListarEmpleadosQuery, Result<IReadOnlyList<EmpleadoDto>>>
{
    private readonly IEmpleadoRepository _empleadoRepository;

    public ListarEmpleadosQueryHandler(IEmpleadoRepository empleadoRepository)
    {
        _empleadoRepository = empleadoRepository;
    }

    public async Task<Result<IReadOnlyList<EmpleadoDto>>> Handle(
        ListarEmpleadosQuery request,
        CancellationToken cancellationToken)
    {
        var empleados = await _empleadoRepository.ListAsync(request.IncludeInactive, cancellationToken);
        var dtos = empleados.Select(EmpleadoMapper.ToDto).ToList();
        return Result<IReadOnlyList<EmpleadoDto>>.Success(dtos);
    }
}

public record ObtenerEmpleadoQuery(Guid Id) : IRequest<Result<EmpleadoDto>>;

public class ObtenerEmpleadoQueryHandler : IRequestHandler<ObtenerEmpleadoQuery, Result<EmpleadoDto>>
{
    private readonly IEmpleadoRepository _empleadoRepository;

    public ObtenerEmpleadoQueryHandler(IEmpleadoRepository empleadoRepository)
    {
        _empleadoRepository = empleadoRepository;
    }

    public async Task<Result<EmpleadoDto>> Handle(ObtenerEmpleadoQuery request, CancellationToken cancellationToken)
    {
        var empleado = await _empleadoRepository.GetByIdIncludingInactiveAsync(request.Id, cancellationToken);
        if (empleado is null)
            return Result<EmpleadoDto>.Failure("NOT_FOUND", "Usuario no encontrado.");

        return Result<EmpleadoDto>.Success(EmpleadoMapper.ToDto(empleado));
    }
}
