using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Common.Interfaces;

public interface ILegalizacionRepository
{
    Task<Legalizacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Legalizacion legalizacion, CancellationToken cancellationToken = default);
    void Update(Legalizacion legalizacion);
}

public interface IEmpleadoRepository
{
    Task<Domain.Core.Entities.Empleado?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Domain.Core.Entities.Empleado?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}

public interface ICatalogoRepository
{
    Task<IReadOnlyList<Domain.Core.Entities.Moneda>> GetMonedasAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Core.Entities.CategoriaGasto>> GetCategoriasAsync(CancellationToken cancellationToken = default);
}
