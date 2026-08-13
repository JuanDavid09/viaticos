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

    public static LegalizacionHistorial Crear(
        Guid legalizacionId,
        EstadoLegalizacion? estadoAnterior,
        EstadoLegalizacion estadoNuevo,
        Guid usuarioId,
        string? comentario = null)
    {
        return new LegalizacionHistorial
        {
            Id = Guid.NewGuid(),
            LegalizacionId = legalizacionId,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = estadoNuevo,
            UsuarioId = usuarioId,
            Comentario = comentario?.Trim()
        };
    }
}
