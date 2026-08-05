using Viaticos.Domain.Common;
using Viaticos.Domain.Legalizaciones.Entities;
using Viaticos.Domain.Legalizaciones.Enums;

namespace Viaticos.Domain.Tests.Viaticos;

public class LegalizacionTests
{
    private readonly Guid _empleadoId = Guid.NewGuid();
    private readonly Guid _monedaId = Guid.NewGuid();
    private readonly Guid _categoriaId = Guid.NewGuid();

    [Fact]
    public void Crear_ConFechasInvalidas_LanzaExcepcion()
    {
        Assert.Throws<DomainException>(() =>
            Legalizacion.Crear(_empleadoId, "Viaje", new DateOnly(2026, 3, 10), new DateOnly(2026, 3, 5), _monedaId, 0, _empleadoId));
    }

    [Fact]
    public void EnviarValidacion_SinGastos_LanzaExcepcion()
    {
        var legalizacion = CrearLegalizacion();

        Assert.Throws<DomainException>(() => legalizacion.EnviarValidacion(_empleadoId));
    }

    [Fact]
    public void FlujoCompleto_HastaCierre_TransicionaEstadosCorrectamente()
    {
        var legalizacion = CrearLegalizacionConGasto();

        legalizacion.EnviarValidacion(_empleadoId);
        Assert.Equal(EstadoLegalizacion.PendienteValidacion, legalizacion.Estado);

        legalizacion.EnviarAprobacion(_empleadoId);
        Assert.Equal(EstadoLegalizacion.PendienteAprobacion, legalizacion.Estado);

        legalizacion.Aprobar(_empleadoId);
        Assert.Equal(EstadoLegalizacion.Aprobada, legalizacion.Estado);

        legalizacion.EnviarNomina(_empleadoId);
        Assert.Equal(EstadoLegalizacion.PendienteNomina, legalizacion.Estado);

        legalizacion.Cerrar(_empleadoId);
        Assert.Equal(EstadoLegalizacion.Cerrada, legalizacion.Estado);
        Assert.NotNull(legalizacion.ClosedAt);
    }

    [Fact]
    public void Rechazar_SinComentario_LanzaExcepcion()
    {
        var legalizacion = CrearLegalizacionConGasto();
        legalizacion.EnviarValidacion(_empleadoId);
        legalizacion.EnviarAprobacion(_empleadoId);

        Assert.Throws<DomainException>(() => legalizacion.Rechazar(_empleadoId, ""));
    }

    [Fact]
    public void Rechazar_ConComentario_PermiteReabrir()
    {
        var legalizacion = CrearLegalizacionConGasto();
        legalizacion.EnviarValidacion(_empleadoId);
        legalizacion.EnviarAprobacion(_empleadoId);
        legalizacion.Rechazar(_empleadoId, "Faltan soportes");

        Assert.Equal(EstadoLegalizacion.Rechazada, legalizacion.Estado);

        legalizacion.Reabrir(_empleadoId);
        Assert.Equal(EstadoLegalizacion.Borrador, legalizacion.Estado);
    }

    private Legalizacion CrearLegalizacion() =>
        Legalizacion.Crear(_empleadoId, "Viaje comercial", new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 5), _monedaId, 500_000, _empleadoId);

    private Legalizacion CrearLegalizacionConGasto()
    {
        var legalizacion = CrearLegalizacion();
        legalizacion.AgregarGasto(_categoriaId, new DateOnly(2026, 3, 2), "Taxi aeropuerto", 45_000, _empleadoId);
        return legalizacion;
    }
}
