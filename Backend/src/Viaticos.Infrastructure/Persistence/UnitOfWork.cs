using Viaticos.Application.Common.Interfaces;

namespace Viaticos.Infrastructure.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ViaticosDbContext _context;

    public UnitOfWork(ViaticosDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
