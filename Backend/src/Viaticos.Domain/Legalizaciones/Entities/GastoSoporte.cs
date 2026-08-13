namespace Viaticos.Domain.Legalizaciones.Entities;

public class GastoSoporte : Entity
{
    public Guid GastoId { get; private set; }
    public Guid ArchivoId { get; private set; }
    public bool EsPrincipal { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private GastoSoporte() { }

    public static GastoSoporte Crear(Guid gastoId, Guid archivoId, Guid createdBy, bool esPrincipal = false)
    {
        return new GastoSoporte
        {
            Id = Guid.NewGuid(),
            GastoId = gastoId,
            ArchivoId = archivoId,
            EsPrincipal = esPrincipal,
            CreatedBy = createdBy
        };
    }
}
