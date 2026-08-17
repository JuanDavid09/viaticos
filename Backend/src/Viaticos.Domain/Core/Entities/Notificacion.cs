namespace Viaticos.Domain.Core.Entities;

using Viaticos.Domain.Common;

public class Notificacion : Entity
{
    public Guid DestinatarioId { get; private set; }
    public string Tipo { get; private set; } = string.Empty;
    public string Titulo { get; private set; } = string.Empty;
    public string Mensaje { get; private set; } = string.Empty;
    public string? EntidadTipo { get; private set; }
    public Guid? EntidadId { get; private set; }
    public bool Leida { get; private set; }
    public DateTime? LeidaAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Notificacion() { }

    public static Notificacion Crear(
        Guid destinatarioId,
        string tipo,
        string titulo,
        string mensaje,
        string? entidadTipo = null,
        Guid? entidadId = null)
    {
        if (destinatarioId == Guid.Empty)
            throw new DomainException("DESTINATARIO_REQUERIDO", "El destinatario es obligatorio.");

        if (string.IsNullOrWhiteSpace(tipo))
            throw new DomainException("TIPO_REQUERIDO", "El tipo de notificación es obligatorio.");

        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("TITULO_REQUERIDO", "El título es obligatorio.");

        if (string.IsNullOrWhiteSpace(mensaje))
            throw new DomainException("MENSAJE_REQUERIDO", "El mensaje es obligatorio.");

        return new Notificacion
        {
            Id = Guid.NewGuid(),
            DestinatarioId = destinatarioId,
            Tipo = tipo.Trim(),
            Titulo = titulo.Trim(),
            Mensaje = mensaje.Trim(),
            EntidadTipo = entidadTipo?.Trim(),
            EntidadId = entidadId,
            Leida = false,
        };
    }

    public void MarcarLeida()
    {
        if (Leida)
            return;

        Leida = true;
        LeidaAt = DateTime.UtcNow;
    }
}
