using Microsoft.EntityFrameworkCore;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Infrastructure.Persistence.Repositories;

internal class LegalizacionRepository : ILegalizacionRepository
{
    private readonly ViaticosDbContext _context;

    public LegalizacionRepository(ViaticosDbContext context)
    {
        _context = context;
    }

    public async Task<Legalizacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Legalizaciones
            .Include(l => l.Gastos)
            .AsSplitQuery()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<Legalizacion?> GetByGastoIdAsync(Guid gastoId, CancellationToken cancellationToken = default)
    {
        return await _context.Legalizaciones
            .Include(l => l.Gastos)
            .AsSplitQuery()
            .FirstOrDefaultAsync(l => l.Gastos.Any(g => g.Id == gastoId), cancellationToken);
    }

    public async Task<IReadOnlyList<Legalizacion>> ListByEmpleadoAsync(Guid empleadoId, CancellationToken cancellationToken = default)
    {
        return await _context.Legalizaciones
            .Where(l => l.EmpleadoId == empleadoId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Legalizacion>> ListPendientesAprobacionAsync(
        Guid? jefeId,
        CancellationToken cancellationToken = default)
    {
        var query =
            from legalizacion in _context.Legalizaciones
            join empleado in _context.Empleados on legalizacion.EmpleadoId equals empleado.Id
            where legalizacion.Estado == Domain.Legalizaciones.Enums.EstadoLegalizacion.PendienteAprobacion
            select new { legalizacion, empleado };

        if (jefeId.HasValue)
            query = query.Where(x => x.empleado.JefeId == jefeId.Value);

        return await query
            .OrderBy(x => x.legalizacion.SubmittedAt)
            .Select(x => x.legalizacion)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Legalizacion>> ListPendientesNominaAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Legalizaciones
            .Where(l => l.Estado == Domain.Legalizaciones.Enums.EstadoLegalizacion.PendienteNomina)
            .OrderBy(l => l.SubmittedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LegalizacionCalendarioEntry>> ListCalendarioAsync(
        Guid? jefeId,
        DateOnly? desde,
        DateOnly? hasta,
        CancellationToken cancellationToken = default)
    {
        var query =
            from legalizacion in _context.Legalizaciones
            join empleado in _context.Empleados on legalizacion.EmpleadoId equals empleado.Id
            join moneda in _context.Monedas on legalizacion.MonedaId equals moneda.Id
            where empleado.Activo
            select new { legalizacion, empleado, moneda };

        if (jefeId.HasValue)
            query = query.Where(x => x.empleado.JefeId == jefeId.Value);

        if (desde.HasValue)
            query = query.Where(x => x.legalizacion.FechaFin >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(x => x.legalizacion.FechaInicio <= hasta.Value);

        var rows = await query
            .OrderBy(x => x.legalizacion.FechaInicio)
            .ThenBy(x => x.empleado.Apellido)
            .ThenBy(x => x.empleado.Nombre)
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => new LegalizacionCalendarioEntry(
                x.legalizacion,
                $"{x.empleado.Nombre} {x.empleado.Apellido}".Trim(),
                string.IsNullOrWhiteSpace(x.moneda.Simbolo)
                    ? x.moneda.CodigoIso.Trim()
                    : x.moneda.Simbolo.Trim()))
            .ToList();
    }

    public async Task<IReadOnlyList<LegalizacionHistorial>> GetHistorialAsync(
        Guid legalizacionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LegalizacionHistorial
            .Where(h => h.LegalizacionId == legalizacionId)
            .OrderBy(h => h.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Legalizacion legalizacion, CancellationToken cancellationToken = default)
    {
        await _context.Legalizaciones.AddAsync(legalizacion, cancellationToken);
    }

    public void Update(Legalizacion legalizacion)
    {
        _context.Legalizaciones.Update(legalizacion);
    }

    public async Task<Gasto> AddGastoAsync(
        Legalizacion legalizacion,
        Guid categoriaGastoId,
        DateOnly fechaGasto,
        string descripcion,
        decimal monto,
        Guid createdBy,
        string? proveedor,
        string? numeroDocumento,
        CancellationToken cancellationToken = default)
    {
        var gasto = legalizacion.AgregarGasto(
            categoriaGastoId,
            fechaGasto,
            descripcion,
            monto,
            createdBy,
            proveedor,
            numeroDocumento);

        foreach (var entry in _context.ChangeTracker.Entries<Legalizacion>().Where(e => e.Entity.Id == legalizacion.Id))
            entry.State = EntityState.Detached;

        foreach (var entry in _context.ChangeTracker.Entries<Gasto>().Where(e => e.Entity.Id != gasto.Id))
            entry.State = EntityState.Detached;

        await _context.Gastos.AddAsync(gasto, cancellationToken);
        return gasto;
    }

    public async Task PersistWorkflowTransitionAsync(
        Legalizacion legalizacion,
        CancellationToken cancellationToken = default)
    {
        foreach (var entry in _context.ChangeTracker.Entries().ToList())
        {
            entry.State = EntityState.Detached;
        }

        await _context.Legalizaciones
            .Where(l => l.Id == legalizacion.Id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(l => l.Estado, legalizacion.Estado)
                    .SetProperty(l => l.UpdatedBy, legalizacion.UpdatedBy)
                    .SetProperty(l => l.SubmittedAt, legalizacion.SubmittedAt)
                    .SetProperty(l => l.ClosedAt, legalizacion.ClosedAt)
                    .SetProperty(l => l.Observaciones, legalizacion.Observaciones),
                cancellationToken);
    }
}
