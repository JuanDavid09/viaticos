using Viaticos.Application.Common.Models;
using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Common.Interfaces;

public interface ILegalizacionWorkflowService
{
    Result EnsureIsOwner(Legalizacion legalizacion, Guid userId);
    Task<Result> EnsureIsJefeDelEmpleadoAsync(Legalizacion legalizacion, Guid jefeId, CancellationToken cancellationToken = default);
}
