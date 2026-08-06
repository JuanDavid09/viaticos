using Microsoft.EntityFrameworkCore;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Core.Entities;

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
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Email == email && e.Activo, cancellationToken);
    }
}
