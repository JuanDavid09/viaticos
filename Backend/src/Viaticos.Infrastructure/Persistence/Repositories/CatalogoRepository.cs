using Microsoft.EntityFrameworkCore;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Core.Entities;

namespace Viaticos.Infrastructure.Persistence.Repositories;

internal class CatalogoRepository : ICatalogoRepository
{
    private readonly ViaticosDbContext _context;

    public CatalogoRepository(ViaticosDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Moneda>> GetMonedasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Monedas
            .Where(m => m.Activo)
            .OrderBy(m => m.CodigoIso)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CategoriaGasto>> GetCategoriasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CategoriasGasto
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }
}
