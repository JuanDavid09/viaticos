using Microsoft.EntityFrameworkCore;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Core.Entities;
using Viaticos.Domain.Core.Enums;

namespace Viaticos.Infrastructure.Persistence.Repositories;

internal class EmpleadoRepository : IEmpleadoRepository
{
    private readonly ViaticosDbContext _context;

    public EmpleadoRepository(ViaticosDbContext context)
    {
        _context = context;
    }

    public async Task<Empleado?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo, cancellationToken);
    }

    public async Task<Empleado?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Email == normalizedEmail && e.Activo, cancellationToken);
    }

    public async Task<Empleado?> GetByIdIncludingInactiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Empleado>> ListAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = _context.Empleados.AsQueryable();
        if (!includeInactive)
            query = query.Where(e => e.Activo);

        return await query
            .OrderBy(e => e.Nombre)
            .ThenBy(e => e.Apellido)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Empleado>> ListActivosByRolAsync(
        Rol rol,
        CancellationToken cancellationToken = default)
    {
        return await _context.Empleados
            .Where(e => e.Activo && e.Rol == rol)
            .OrderBy(e => e.Nombre)
            .ThenBy(e => e.Apellido)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var query = _context.Empleados.Where(e => e.Email == normalizedEmail);
        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodigoAsync(
        string codigoEmpleado,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCodigo = codigoEmpleado.Trim();
        var query = _context.Empleados.Where(e => e.CodigoEmpleado == normalizedCodigo);
        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Empleado empleado, CancellationToken cancellationToken = default)
    {
        await _context.Empleados.AddAsync(empleado, cancellationToken);
    }

    public void Update(Empleado empleado)
    {
        _context.Empleados.Update(empleado);
    }
}
