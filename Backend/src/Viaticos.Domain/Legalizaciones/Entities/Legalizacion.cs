namespace Viaticos.Domain.Legalizaciones.Entities;

using Viaticos.Domain.Legalizaciones.Enums;

/// <summary>
/// Agregado raíz del módulo de legalizaciones.
/// Encapsula el ciclo de vida y las transiciones de estado.
/// </summary>
public class Legalizacion : AggregateRoot
{
    private readonly List<Gasto> _gastos = [];

    public string Numero { get; private set; } = string.Empty;
    public Guid EmpleadoId { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public string? Destino { get; private set; }
    public DateOnly FechaInicio { get; private set; }
    public DateOnly FechaFin { get; private set; }
    public Guid MonedaId { get; private set; }
    public decimal MontoAnticipo { get; private set; }
    public EstadoLegalizacion Estado { get; private set; }
    public decimal TotalGastos { get; private set; }
    public decimal TotalReembolso { get; private set; }
    public decimal TotalDevolucion { get; private set; }
    public string? Observaciones { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    public IReadOnlyCollection<Gasto> Gastos => _gastos.AsReadOnly();

    private Legalizacion() { }

    public static Legalizacion Crear(
        Guid empleadoId,
        string motivo,
        DateOnly fechaInicio,
        DateOnly fechaFin,
        Guid monedaId,
        decimal montoAnticipo,
        Guid createdBy)
    {
        if (fechaFin < fechaInicio)
            throw new DomainException("FECHAS_INVALIDAS", "La fecha fin debe ser mayor o igual a la fecha inicio.");

        if (montoAnticipo < 0)
            throw new DomainException("ANTICIPO_INVALIDO", "El anticipo no puede ser negativo.");

        return new Legalizacion
        {
            Id = Guid.NewGuid(),
            EmpleadoId = empleadoId,
            Motivo = motivo,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            MonedaId = monedaId,
            MontoAnticipo = montoAnticipo,
            Estado = EstadoLegalizacion.Borrador,
            CreatedBy = createdBy
        };
    }

    public Gasto AgregarGasto(
        Guid categoriaGastoId,
        DateOnly fechaGasto,
        string descripcion,
        decimal monto,
        Guid createdBy,
        string? proveedor = null,
        string? numeroDocumento = null)
    {
        EnsureEditable();

        var gasto = Gasto.Crear(Id, categoriaGastoId, fechaGasto, descripcion, monto, createdBy, proveedor, numeroDocumento);
        _gastos.Add(gasto);
        return gasto;
    }

    public void EnviarValidacion(Guid usuarioId)
    {
        EnsureEstado(EstadoLegalizacion.Borrador);
        if (_gastos.Count == 0)
            throw new DomainException("SIN_GASTOS", "Debe registrar al menos un gasto.");

        Transicionar(EstadoLegalizacion.PendienteValidacion, usuarioId);
    }

    public void EnviarAprobacion(Guid usuarioId)
    {
        EnsureEstado(EstadoLegalizacion.PendienteValidacion);
        Transicionar(EstadoLegalizacion.PendienteAprobacion, usuarioId);
        SubmittedAt = DateTime.UtcNow;
    }

    public void Aprobar(Guid aprobadorId)
    {
        EnsureEstado(EstadoLegalizacion.PendienteAprobacion);
        Transicionar(EstadoLegalizacion.Aprobada, aprobadorId);
    }

    public void Rechazar(Guid aprobadorId, string comentario)
    {
        EnsureEstado(EstadoLegalizacion.PendienteAprobacion);
        if (string.IsNullOrWhiteSpace(comentario))
            throw new DomainException("COMENTARIO_REQUERIDO", "Debe indicar el motivo del rechazo.");

        Observaciones = comentario;
        Transicionar(EstadoLegalizacion.Rechazada, aprobadorId);
    }

    public void EnviarNomina(Guid usuarioId)
    {
        EnsureEstado(EstadoLegalizacion.Aprobada);
        Transicionar(EstadoLegalizacion.PendienteNomina, usuarioId);
    }

    public void Cerrar(Guid usuarioId)
    {
        EnsureEstado(EstadoLegalizacion.PendienteNomina);
        Transicionar(EstadoLegalizacion.Cerrada, usuarioId);
        ClosedAt = DateTime.UtcNow;
    }

    public void Reabrir(Guid usuarioId)
    {
        EnsureEstado(EstadoLegalizacion.Rechazada);
        Transicionar(EstadoLegalizacion.Borrador, usuarioId);
    }

    private void EnsureEditable()
    {
        if (Estado is not (EstadoLegalizacion.Borrador or EstadoLegalizacion.PendienteValidacion))
            throw new DomainException("NO_EDITABLE", $"No se puede modificar en estado {Estado}.");
    }

    private void EnsureEstado(EstadoLegalizacion esperado)
    {
        if (Estado != esperado)
            throw new DomainException("ESTADO_INVALIDO", $"Operación no permitida en estado {Estado}. Se esperaba {esperado}.");
    }

    private void Transicionar(EstadoLegalizacion nuevoEstado, Guid usuarioId)
    {
        Estado = nuevoEstado;
        UpdatedBy = usuarioId;
    }
}
