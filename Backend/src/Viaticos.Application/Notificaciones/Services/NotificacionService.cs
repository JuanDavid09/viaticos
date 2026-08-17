using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Notificaciones;
using Viaticos.Domain.Core.Entities;
using Viaticos.Domain.Core.Enums;
using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Notificaciones.Services;

public class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificacionService(
        INotificacionRepository notificacionRepository,
        IEmpleadoRepository empleadoRepository,
        IUnitOfWork unitOfWork)
    {
        _notificacionRepository = notificacionRepository;
        _empleadoRepository = empleadoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task NotificarLegalizacionCreadaAsync(
        Legalizacion legalizacion,
        string empleadoNombre,
        Guid actorId,
        CancellationToken cancellationToken = default)
    {
        var referencia = BuildReferencia(legalizacion);
        var titulo = "Nueva legalización creada";
        var mensaje =
            $"{empleadoNombre} creó la legalización {referencia} ({legalizacion.Motivo}). " +
            $"Viaje del {FormatDate(legalizacion.FechaInicio)} al {FormatDate(legalizacion.FechaFin)}.";

        await DispatchAsync(
            await BuildSupervisoresRecipients(legalizacion.EmpleadoId, actorId, cancellationToken),
            NotificacionTipos.LegalizacionCreada,
            titulo,
            mensaje,
            legalizacion.Id,
            cancellationToken);
    }

    public async Task NotificarGastoAgregadoAsync(
        Legalizacion legalizacion,
        string empleadoNombre,
        string gastoDescripcion,
        decimal gastoMonto,
        Guid actorId,
        CancellationToken cancellationToken = default)
    {
        var referencia = BuildReferencia(legalizacion);
        var titulo = "Gasto agregado a legalización";
        var mensaje =
            $"{empleadoNombre} registró un gasto de {gastoMonto:N0} en {referencia}: {gastoDescripcion}.";

        await DispatchAsync(
            await BuildSupervisoresRecipients(legalizacion.EmpleadoId, actorId, cancellationToken),
            NotificacionTipos.GastoAgregado,
            titulo,
            mensaje,
            legalizacion.Id,
            cancellationToken);
    }

    public async Task NotificarTransicionWorkflowAsync(
        Legalizacion legalizacion,
        string empleadoNombre,
        string evento,
        Guid actorId,
        string? detalle = null,
        CancellationToken cancellationToken = default)
    {
        var referencia = BuildReferencia(legalizacion);
        var recipients = new HashSet<Guid>();

        switch (evento)
        {
            case NotificacionTipos.EnviadaValidacion:
                await DispatchAsync(
                    new[] { legalizacion.EmpleadoId },
                    evento,
                    "Legalización en validación",
                    $"Tu legalización {referencia} fue enviada a validación interna.",
                    legalizacion.Id,
                    cancellationToken);
                await DispatchAsync(
                    await BuildSupervisoresRecipients(legalizacion.EmpleadoId, actorId, cancellationToken),
                    evento,
                    "Legalización enviada a validación",
                    $"{empleadoNombre} envió {referencia} a validación interna.",
                    legalizacion.Id,
                    cancellationToken);
                break;

            case NotificacionTipos.EnviadaAprobacion:
                await DispatchAsync(
                    new[] { legalizacion.EmpleadoId },
                    evento,
                    "Enviada a aprobación",
                    $"Tu legalización {referencia} fue enviada a aprobación del jefe.",
                    legalizacion.Id,
                    cancellationToken);
                await DispatchAsync(
                    await BuildSupervisoresRecipients(legalizacion.EmpleadoId, actorId, cancellationToken),
                    evento,
                    "Legalización pendiente de aprobación",
                    $"{empleadoNombre} envió {referencia} para aprobación del jefe.",
                    legalizacion.Id,
                    cancellationToken);
                break;

            case NotificacionTipos.Aprobada:
                AddOwner(recipients, legalizacion.EmpleadoId);
                await DispatchAsync(
                    recipients,
                    evento,
                    "Legalización aprobada",
                    $"Tu legalización {referencia} fue aprobada.",
                    legalizacion.Id,
                    cancellationToken);
                break;

            case NotificacionTipos.Rechazada:
                AddOwner(recipients, legalizacion.EmpleadoId);
                await DispatchAsync(
                    recipients,
                    evento,
                    "Legalización rechazada",
                    $"Tu legalización {referencia} fue rechazada. Motivo: {detalle ?? "Sin comentario"}.",
                    legalizacion.Id,
                    cancellationToken);
                break;

            case NotificacionTipos.Reabierta:
                AddOwner(recipients, legalizacion.EmpleadoId);
                await AddSupervisoresAsync(recipients, legalizacion.EmpleadoId, actorId, cancellationToken);
                await DispatchAsync(
                    recipients,
                    evento,
                    "Legalización reabierta",
                    $"{empleadoNombre} reabrió {referencia} como borrador para correcciones.",
                    legalizacion.Id,
                    cancellationToken);
                break;

            case NotificacionTipos.EnviadaNomina:
                await DispatchAsync(
                    new[] { legalizacion.EmpleadoId },
                    evento,
                    "Enviada a nómina",
                    $"Tu legalización {referencia} fue enviada al área de nómina.",
                    legalizacion.Id,
                    cancellationToken);
                var nominaRecipients = new HashSet<Guid>();
                await AddByRolAsync(nominaRecipients, Rol.Nomina, actorId, cancellationToken);
                await DispatchAsync(
                    nominaRecipients,
                    evento,
                    "Legalización pendiente de cierre",
                    $"{referencia} de {empleadoNombre} está lista para cierre de nómina.",
                    legalizacion.Id,
                    cancellationToken);
                break;

            case NotificacionTipos.Cerrada:
                AddOwner(recipients, legalizacion.EmpleadoId);
                await DispatchAsync(
                    recipients,
                    evento,
                    "Legalización cerrada",
                    $"Tu legalización {referencia} fue cerrada por nómina.",
                    legalizacion.Id,
                    cancellationToken);
                break;
        }
    }

    private async Task<IReadOnlyCollection<Guid>> BuildSupervisoresRecipients(
        Guid empleadoId,
        Guid actorId,
        CancellationToken cancellationToken)
    {
        var recipients = new HashSet<Guid>();
        await AddSupervisoresAsync(recipients, empleadoId, actorId, cancellationToken);
        return recipients;
    }

    private async Task AddSupervisoresAsync(
        HashSet<Guid> recipients,
        Guid empleadoId,
        Guid actorId,
        CancellationToken cancellationToken)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(empleadoId, cancellationToken);
        if (empleado?.JefeId is Guid jefeId && jefeId != actorId)
            recipients.Add(jefeId);

        await AddByRolAsync(recipients, Rol.Admin, actorId, cancellationToken);
    }

    private async Task AddByRolAsync(
        HashSet<Guid> recipients,
        Rol rol,
        Guid actorId,
        CancellationToken cancellationToken)
    {
        var usuarios = await _empleadoRepository.ListActivosByRolAsync(rol, cancellationToken);
        foreach (var usuario in usuarios)
        {
            if (usuario.Id == actorId)
                continue;

            recipients.Add(usuario.Id);
        }
    }

    private static void AddOwner(HashSet<Guid> recipients, Guid empleadoId)
    {
        recipients.Add(empleadoId);
    }

    private async Task DispatchAsync(
        IEnumerable<Guid> destinatarios,
        string tipo,
        string titulo,
        string mensaje,
        Guid legalizacionId,
        CancellationToken cancellationToken)
    {
        var notificaciones = destinatarios
            .Distinct()
            .Select(destinatarioId => Notificacion.Crear(
                destinatarioId,
                tipo,
                titulo,
                mensaje,
                NotificacionTipos.EntidadLegalizacion,
                legalizacionId))
            .ToList();

        if (notificaciones.Count == 0)
            return;

        await _notificacionRepository.AddRangeAsync(notificaciones, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string BuildReferencia(Legalizacion legalizacion) =>
        string.IsNullOrWhiteSpace(legalizacion.Numero)
            ? legalizacion.Motivo
            : legalizacion.Numero;

    private static string FormatDate(DateOnly date) => date.ToString("dd/MM/yyyy");
}
