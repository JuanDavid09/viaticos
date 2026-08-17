using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Common.Interfaces;

public record LegalizacionCalendarioEntry(
    Legalizacion Legalizacion,
    string EmpleadoNombre,
    string MonedaSimbolo);

public interface ILegalizacionRepository
{
    Task<Legalizacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Legalizacion?> GetByGastoIdAsync(Guid gastoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Legalizacion>> ListByEmpleadoAsync(Guid empleadoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Legalizacion>> ListPendientesAprobacionByJefeAsync(Guid jefeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Legalizacion>> ListPendientesNominaAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LegalizacionCalendarioEntry>> ListCalendarioAsync(
        Guid? jefeId,
        DateOnly? desde,
        DateOnly? hasta,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LegalizacionHistorial>> GetHistorialAsync(Guid legalizacionId, CancellationToken cancellationToken = default);
    Task AddAsync(Legalizacion legalizacion, CancellationToken cancellationToken = default);
    void Update(Legalizacion legalizacion);
}

public interface IEmpleadoRepository
{
    Task<Domain.Core.Entities.Empleado?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Domain.Core.Entities.Empleado?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Domain.Core.Entities.Empleado?> GetByIdIncludingInactiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Core.Entities.Empleado>> ListAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Core.Entities.Empleado>> ListActivosByRolAsync(
        Domain.Core.Enums.Rol rol,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Core.Entities.Empleado>> ListAsignablesLegalizacionAsync(
        Guid? jefeId,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodigoAsync(string codigoEmpleado, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Core.Entities.Empleado empleado, CancellationToken cancellationToken = default);
    void Update(Domain.Core.Entities.Empleado empleado);
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

public interface INotificacionRepository
{
    Task<IReadOnlyList<Domain.Core.Entities.Notificacion>> ListByDestinatarioAsync(
        Guid destinatarioId,
        int limite,
        CancellationToken cancellationToken = default);
    Task<int> CountNoLeidasAsync(Guid destinatarioId, CancellationToken cancellationToken = default);
    Task<Domain.Core.Entities.Notificacion?> GetByIdForDestinatarioAsync(
        Guid id,
        Guid destinatarioId,
        CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Domain.Core.Entities.Notificacion> notificaciones, CancellationToken cancellationToken = default);
    Task MarcarTodasLeidasAsync(Guid destinatarioId, CancellationToken cancellationToken = default);
}
