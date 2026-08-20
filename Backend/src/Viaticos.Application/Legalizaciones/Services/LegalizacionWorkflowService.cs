using Viaticos.Application.Legalizaciones;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Domain.Legalizaciones.Entities;
using Viaticos.Domain.Legalizaciones.Enums;

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

    public async Task<Result> CanViewLegalizacionAsync(
        Legalizacion legalizacion,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken = default)
    {
        if (legalizacion.EmpleadoId == currentUser.UserId)
            return Result.Success();

        if (currentUser.IsInRole("Admin") || currentUser.IsInRole("Nomina"))
            return Result.Success();

        if (currentUser.IsInRole("JefeAprobador"))
            return await EnsureIsJefeDelEmpleadoAsync(legalizacion, currentUser.UserId, cancellationToken);

        return Result.Failure("FORBIDDEN", "No tiene permiso para ver esta legalización.");
    }

    public async Task<IReadOnlyList<string>> GetAvailableActionsAsync(
        Legalizacion legalizacion,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken = default)
    {
        var actions = new List<string>();
        var isOwner = legalizacion.EmpleadoId == currentUser.UserId;

        if (isOwner)
        {
            switch (legalizacion.Estado)
            {
                case EstadoLegalizacion.Borrador when legalizacion.Gastos.Count > 0:
                    actions.Add(LegalizacionWorkflowActions.EnviarValidacion);
                    break;
                case EstadoLegalizacion.PendienteValidacion:
                    actions.Add(LegalizacionWorkflowActions.EnviarAprobacion);
                    break;
                case EstadoLegalizacion.Rechazada:
                    actions.Add(LegalizacionWorkflowActions.Reabrir);
                    break;
                case EstadoLegalizacion.Aprobada:
                    actions.Add(LegalizacionWorkflowActions.EnviarNomina);
                    break;
            }
        }

        if (legalizacion.Estado == EstadoLegalizacion.PendienteAprobacion)
        {
            if (currentUser.IsInRole("Admin"))
            {
                actions.Add(LegalizacionWorkflowActions.Aprobar);
                actions.Add(LegalizacionWorkflowActions.Rechazar);
            }
            else if (currentUser.IsInRole("JefeAprobador") && !isOwner)
            {
                var jefeAuth = await EnsureIsJefeDelEmpleadoAsync(
                    legalizacion,
                    currentUser.UserId,
                    cancellationToken);
                if (jefeAuth.IsSuccess)
                {
                    actions.Add(LegalizacionWorkflowActions.Aprobar);
                    actions.Add(LegalizacionWorkflowActions.Rechazar);
                }
            }
        }

        if (legalizacion.Estado == EstadoLegalizacion.PendienteNomina
            && (currentUser.IsInRole("Nomina") || currentUser.IsInRole("Admin")))
        {
            actions.Add(LegalizacionWorkflowActions.Cerrar);
        }

        return actions;
    }
}
