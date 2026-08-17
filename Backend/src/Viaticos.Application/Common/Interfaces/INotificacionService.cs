using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Common.Interfaces;

public interface INotificacionService
{
    Task NotificarLegalizacionCreadaAsync(
        Legalizacion legalizacion,
        string empleadoNombre,
        Guid actorId,
        CancellationToken cancellationToken = default);

    Task NotificarGastoAgregadoAsync(
        Legalizacion legalizacion,
        string empleadoNombre,
        string gastoDescripcion,
        decimal gastoMonto,
        Guid actorId,
        CancellationToken cancellationToken = default);

    Task NotificarTransicionWorkflowAsync(
        Legalizacion legalizacion,
        string empleadoNombre,
        string evento,
        Guid actorId,
        string? detalle = null,
        CancellationToken cancellationToken = default);
}
