namespace Viaticos.Domain.Legalizaciones.Entities;

using Viaticos.Domain.Legalizaciones.Enums;

public class LegalizacionHistorial : Entity
{
    public Guid LegalizacionId { get; private set; }
    public EstadoLegalizacion? EstadoAnterior { get; private set; }
    public EstadoLegalizacion EstadoNuevo { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string? Comentario { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private LegalizacionHistorial() { }
}
