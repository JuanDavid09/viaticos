namespace Viaticos.Domain.Core.Entities;

public class Moneda : Entity
{
    public string CodigoIso { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string? Simbolo { get; private set; }
    public bool Activo { get; private set; }

    private Moneda() { }
}

public class CategoriaGasto : Entity
{
    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public bool RequiereSoporte { get; private set; }
    public bool Activo { get; private set; }

    private CategoriaGasto() { }
}
