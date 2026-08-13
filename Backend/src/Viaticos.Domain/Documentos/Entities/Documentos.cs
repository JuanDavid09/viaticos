namespace Viaticos.Domain.Documentos.Entities;

using Viaticos.Domain.Common;
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

    public static Archivo Crear(
        string bucket,
        string objectKey,
        string nombreOriginal,
        string mimeType,
        long tamanoBytes,
        Guid subidoPor)
    {
        if (tamanoBytes <= 0)
            throw new DomainException("ARCHIVO_INVALIDO", "El archivo debe tener contenido.");

        if (string.IsNullOrWhiteSpace(nombreOriginal))
            throw new DomainException("ARCHIVO_INVALIDO", "El nombre del archivo es obligatorio.");

        return new Archivo
        {
            Id = Guid.NewGuid(),
            Bucket = bucket,
            ObjectKey = objectKey,
            NombreOriginal = nombreOriginal.Trim(),
            MimeType = mimeType,
            TamanoBytes = tamanoBytes,
            SubidoPor = subidoPor
        };
    }
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

    public static OcrExtraccion Crear(Guid archivoId, Guid gastoSoporteId)
    {
        return new OcrExtraccion
        {
            Id = Guid.NewGuid(),
            ArchivoId = archivoId,
            GastoSoporteId = gastoSoporteId,
            Estado = EstadoOcr.Pendiente
        };
    }

    public void MarcarProcesando()
    {
        if (Estado is not EstadoOcr.Pendiente and not EstadoOcr.Error)
            throw new DomainException("OCR_ESTADO_INVALIDO", "La extracción OCR no puede procesarse en su estado actual.");

        Estado = EstadoOcr.Procesando;
        ErrorMensaje = null;
    }

    public void Completar(string rawJson, IEnumerable<(string NombreCampo, string? Valor)> campos, string? azureOperationId = null)
    {
        if (Estado is not EstadoOcr.Procesando)
            throw new DomainException("OCR_ESTADO_INVALIDO", "La extracción OCR no está en procesamiento.");

        _campos.Clear();
        foreach (var (nombreCampo, valor) in campos)
            _campos.Add(OcrCampo.Crear(Id, nombreCampo, valor));

        JsonRespuesta = rawJson;
        AzureOperationId = azureOperationId;
        Estado = EstadoOcr.Completado;
        ProcesadoAt = DateTime.UtcNow;
        ErrorMensaje = null;
    }

    public void MarcarError(string mensaje)
    {
        Estado = EstadoOcr.Error;
        ErrorMensaje = mensaje;
        ProcesadoAt = DateTime.UtcNow;
    }

    public void ValidarCampos(IEnumerable<(Guid CampoId, string ValorValidado)> campos, Guid validadoPor)
    {
        if (Estado is not EstadoOcr.Completado and not EstadoOcr.ValidadoUsuario)
            throw new DomainException("OCR_ESTADO_INVALIDO", "Solo se pueden validar campos de una extracción completada.");

        foreach (var (campoId, valorValidado) in campos)
        {
            var campo = _campos.FirstOrDefault(c => c.Id == campoId)
                ?? throw new DomainException("CAMPO_NOT_FOUND", $"Campo OCR {campoId} no encontrado.");

            campo.Validar(valorValidado, validadoPor);
        }

        Estado = EstadoOcr.ValidadoUsuario;
    }

    public IReadOnlyDictionary<string, string> ObtenerValoresValidados()
    {
        return _campos
            .Where(c => c.Validado || !string.IsNullOrWhiteSpace(c.ValorExtraido))
            .ToDictionary(
                c => c.NombreCampo,
                c => c.Validado ? c.ValorValidado! : c.ValorExtraido!,
                StringComparer.OrdinalIgnoreCase);
    }
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

    internal static OcrCampo Crear(Guid ocrExtraccionId, string nombreCampo, string? valorExtraido)
    {
        return new OcrCampo
        {
            Id = Guid.NewGuid(),
            OcrExtraccionId = ocrExtraccionId,
            NombreCampo = nombreCampo,
            ValorExtraido = valorExtraido
        };
    }

    public void Validar(string valorValidado, Guid validadoPor)
    {
        if (string.IsNullOrWhiteSpace(valorValidado))
            throw new DomainException("VALOR_INVALIDO", $"El valor validado para '{NombreCampo}' es obligatorio.");

        ValorValidado = valorValidado.Trim();
        Validado = true;
        ValidadoPor = validadoPor;
        ValidadoAt = DateTime.UtcNow;
    }
}
