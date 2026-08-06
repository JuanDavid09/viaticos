namespace Viaticos.Domain.Legalizaciones.Entities;

public class Gasto : Entity
{
    public Guid LegalizacionId { get; private set; }
    public Guid CategoriaGastoId { get; private set; }
    public DateOnly FechaGasto { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public string? Proveedor { get; private set; }
    public string? NumeroDocumento { get; private set; }
    public decimal Monto { get; private set; }
    public bool Validado { get; private set; }
    public Guid? ValidadoPor { get; private set; }
    public DateTime? ValidadoAt { get; private set; }
    public short Orden { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid CreatedBy { get; private set; }

    private Gasto() { }

    internal static Gasto Crear(
        Guid legalizacionId,
        Guid categoriaGastoId,
        DateOnly fechaGasto,
        string descripcion,
        decimal monto,
        Guid createdBy,
        short orden,
        string? proveedor = null,
        string? numeroDocumento = null)
    {
        if (monto <= 0)
            throw new DomainException("MONTO_INVALIDO", "El monto debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("DESCRIPCION_REQUERIDA", "La descripción es obligatoria.");

        return new Gasto
        {
            Id = Guid.NewGuid(),
            LegalizacionId = legalizacionId,
            CategoriaGastoId = categoriaGastoId,
            FechaGasto = fechaGasto,
            Descripcion = descripcion,
            Monto = monto,
            Proveedor = proveedor,
            NumeroDocumento = numeroDocumento,
            Orden = orden,
            CreatedBy = createdBy
        };
    }

    public void MarcarValidado(Guid usuarioId)
    {
        Validado = true;
        ValidadoPor = usuarioId;
        ValidadoAt = DateTime.UtcNow;
    }
}
