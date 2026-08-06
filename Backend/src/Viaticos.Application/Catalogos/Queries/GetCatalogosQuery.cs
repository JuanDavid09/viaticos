using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;

namespace Viaticos.Application.Catalogos.Queries;

public record GetCatalogosQuery : IRequest<Result<CatalogosDto>>;

public class GetCatalogosQueryHandler : IRequestHandler<GetCatalogosQuery, Result<CatalogosDto>>
{
    private readonly ICatalogoRepository _catalogoRepository;

    public GetCatalogosQueryHandler(ICatalogoRepository catalogoRepository)
    {
        _catalogoRepository = catalogoRepository;
    }

    public async Task<Result<CatalogosDto>> Handle(GetCatalogosQuery request, CancellationToken cancellationToken)
    {
        var monedas = await _catalogoRepository.GetMonedasAsync(cancellationToken);
        var categorias = await _catalogoRepository.GetCategoriasAsync(cancellationToken);

        var dto = new CatalogosDto(
            monedas.Select(LegalizacionMapper.ToDto).ToList(),
            categorias.Select(LegalizacionMapper.ToDto).ToList());

        return Result<CatalogosDto>.Success(dto);
    }
}
