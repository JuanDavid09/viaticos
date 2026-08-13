using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Legalizaciones.Services;

public class LegalizacionWorkflowService : ILegalizacionWorkflowService
{
    private readonly IEmpleadoRepository _empleadoRepository;

    public LegalizacionWorkflowService(IEmpleadoRepository empleadoRepository)
    {
        _empleadoRepository = empleadoRepository;
    }

    public Result EnsureIsOwner(Legalizacion legalizacion, Guid userId)
    {
        if (legalizacion.EmpleadoId != userId)
            return Result.Failure("FORBIDDEN", "Solo el propietario puede realizar esta acción.");

        return Result.Success();
    }

    public async Task<Result> EnsureIsJefeDelEmpleadoAsync(
        Legalizacion legalizacion,
        Guid jefeId,
        CancellationToken cancellationToken = default)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(legalizacion.EmpleadoId, cancellationToken);
        if (empleado is null)
            return Result.Failure("NOT_FOUND", "Empleado no encontrado.");

        if (empleado.JefeId != jefeId)
            return Result.Failure("FORBIDDEN", "No es el jefe aprobador de este empleado.");

        return Result.Success();
    }
}
