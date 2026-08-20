using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Documentos.DTOs;
using Viaticos.Application.Legalizaciones.DTOs;
using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Legalizaciones.Services;

public interface ILegalizacionDetalleFactory
{
    Task<LegalizacionDetalleDto> CreateAsync(
        Legalizacion legalizacion,
        IReadOnlyList<GastoSoporteDetalle>? soportes = null,
        CancellationToken cancellationToken = default);
}

public class LegalizacionDetalleFactory : ILegalizacionDetalleFactory
{
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;

    public LegalizacionDetalleFactory(
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser)
    {
        _workflow = workflow;
        _currentUser = currentUser;
    }

    public async Task<LegalizacionDetalleDto> CreateAsync(
        Legalizacion legalizacion,
        IReadOnlyList<GastoSoporteDetalle>? soportes = null,
        CancellationToken cancellationToken = default)
    {
        var acciones = await _workflow.GetAvailableActionsAsync(legalizacion, _currentUser, cancellationToken);
        return LegalizacionMapper.ToDetalle(legalizacion, soportes, acciones);
    }
}
