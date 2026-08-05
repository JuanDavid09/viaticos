namespace Viaticos.Domain.Documentos.Entities;

using Viaticos.Domain.Documentos.Enums;

public class Archivo : AggregateRoot
{
    public string Bucket { get; private set; } = string.Empty;
    public string ObjectKey { get; private set; } = string.Empty;
    public string NombreOriginal { get; private set; } = string.Empty;
    public string MimeType { get; private set; } = string.Empty;
    public long TamanoBytes { get; private set; }
    public Guid SubidoPor { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Archivo() { }
}

public class OcrExtraccion : Entity
{
    public Guid ArchivoId { get; private set; }
    public Guid? GastoSoporteId { get; private set; }
    public string? AzureOperationId { get; private set; }
    public EstadoOcr Estado { get; private set; }
    public string? JsonRespuesta { get; private set; }
    public string? ErrorMensaje { get; private set; }
    public DateTime? ProcesadoAt { get; private set; }

    private readonly List<OcrCampo> _campos = [];
    public IReadOnlyCollection<OcrCampo> Campos => _campos.AsReadOnly();

    private OcrExtraccion() { }
}

public class OcrCampo : Entity
{
    public Guid OcrExtraccionId { get; private set; }
    public string NombreCampo { get; private set; } = string.Empty;
    public string? ValorExtraido { get; private set; }
    public string? ValorValidado { get; private set; }
    public bool Validado { get; private set; }
    public Guid? ValidadoPor { get; private set; }
    public DateTime? ValidadoAt { get; private set; }

    private OcrCampo() { }
}
