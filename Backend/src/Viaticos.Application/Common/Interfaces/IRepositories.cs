using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Common.Interfaces;

public interface ILegalizacionRepository
{
    Task<Legalizacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Legalizacion?> GetByGastoIdAsync(Guid gastoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Legalizacion>> ListByEmpleadoAsync(Guid empleadoId, CancellationToken cancellationToken = default);
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

public interface IDocumentoRepository
{
    Task<Domain.Documentos.Entities.Archivo?> GetArchivoByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Domain.Legalizaciones.Entities.GastoSoporte?> GetGastoSoporteByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Domain.Documentos.Entities.OcrExtraccion?> GetOcrExtraccionByGastoSoporteIdAsync(Guid gastoSoporteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GastoSoporteDetalle>> ListSoportesByGastoIdsAsync(IEnumerable<Guid> gastoIds, CancellationToken cancellationToken = default);
    Task AddArchivoAsync(Domain.Documentos.Entities.Archivo archivo, CancellationToken cancellationToken = default);
    Task AddGastoSoporteAsync(Domain.Legalizaciones.Entities.GastoSoporte soporte, CancellationToken cancellationToken = default);
    Task AddOcrExtraccionAsync(Domain.Documentos.Entities.OcrExtraccion extraccion, CancellationToken cancellationToken = default);
    void UpdateOcrExtraccion(Domain.Documentos.Entities.OcrExtraccion extraccion);
}

public record GastoSoporteDetalle(
    Guid Id,
    Guid GastoId,
    Guid ArchivoId,
    string NombreOriginal,
    string MimeType,
    long TamanoBytes,
    bool EsPrincipal,
    Guid? OcrExtraccionId,
    string? OcrEstado);
