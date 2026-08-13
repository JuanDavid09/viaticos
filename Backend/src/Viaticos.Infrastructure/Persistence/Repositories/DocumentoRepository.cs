using Microsoft.EntityFrameworkCore;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Documentos.Entities;
using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Infrastructure.Persistence.Repositories;

internal class DocumentoRepository : IDocumentoRepository
{
    private readonly ViaticosDbContext _context;

    public DocumentoRepository(ViaticosDbContext context)
    {
        _context = context;
    }

    public async Task<Archivo?> GetArchivoByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Archivos.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<GastoSoporte?> GetGastoSoporteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GastosSoporte.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<OcrExtraccion?> GetOcrExtraccionByGastoSoporteIdAsync(Guid gastoSoporteId, CancellationToken cancellationToken = default)
    {
        return await _context.OcrExtracciones
            .Include(o => o.Campos)
            .FirstOrDefaultAsync(o => o.GastoSoporteId == gastoSoporteId, cancellationToken);
    }

    public async Task<IReadOnlyList<GastoSoporteDetalle>> ListSoportesByGastoIdsAsync(
        IEnumerable<Guid> gastoIds,
        CancellationToken cancellationToken = default)
    {
        var ids = gastoIds.ToList();
        if (ids.Count == 0)
            return [];

        return await (
            from soporte in _context.GastosSoporte
            join archivo in _context.Archivos on soporte.ArchivoId equals archivo.Id
            join ocr in _context.OcrExtracciones on soporte.Id equals ocr.GastoSoporteId into ocrs
            from ocr in ocrs.DefaultIfEmpty()
            where ids.Contains(soporte.GastoId)
            orderby soporte.CreatedAt
            select new GastoSoporteDetalle(
                soporte.Id,
                soporte.GastoId,
                soporte.ArchivoId,
                archivo.NombreOriginal,
                archivo.MimeType,
                archivo.TamanoBytes,
                soporte.EsPrincipal,
                ocr != null ? ocr.Id : (Guid?)null,
                ocr != null ? ocr.Estado.ToString() : null))
            .ToListAsync(cancellationToken);
    }

    public async Task AddArchivoAsync(Archivo archivo, CancellationToken cancellationToken = default)
    {
        await _context.Archivos.AddAsync(archivo, cancellationToken);
    }

    public async Task AddGastoSoporteAsync(GastoSoporte soporte, CancellationToken cancellationToken = default)
    {
        await _context.GastosSoporte.AddAsync(soporte, cancellationToken);
    }

    public async Task AddOcrExtraccionAsync(OcrExtraccion extraccion, CancellationToken cancellationToken = default)
    {
        await _context.OcrExtracciones.AddAsync(extraccion, cancellationToken);
    }

    public void UpdateOcrExtraccion(OcrExtraccion extraccion)
    {
        _context.OcrExtracciones.Update(extraccion);
    }
}
