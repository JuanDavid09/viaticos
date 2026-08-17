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

    public async Task<IReadOnlyList<Legalizacion>> ListPendientesAprobacionByJefeAsync(
        Guid jefeId,
        CancellationToken cancellationToken = default)
    {
        return await (
            from legalizacion in _context.Legalizaciones
            join empleado in _context.Empleados on legalizacion.EmpleadoId equals empleado.Id
            where empleado.JefeId == jefeId
                  && legalizacion.Estado == Domain.Legalizaciones.Enums.EstadoLegalizacion.PendienteAprobacion
            orderby legalizacion.SubmittedAt
            select legalizacion)
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
}
