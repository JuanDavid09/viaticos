using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Domain.Legalizaciones.Enums;

namespace Viaticos.Application.Reportes.Queries;

public record ReporteFiltrosRequest(
    DateOnly? Desde = null,
    DateOnly? Hasta = null,
    Guid? EmpleadoId = null,
    Guid? JefeId = null,
    string? Departamento = null,
    EstadoLegalizacion? Estado = null,
    int? Anio = null,
    bool SoloCerradas = true);

public record GetResumenPorEstadoReporteQuery(ReporteFiltrosRequest Filtros)
    : IRequest<Result<IReadOnlyList<ResumenPorEstadoDto>>>;

public class GetResumenPorEstadoReporteQueryHandler
    : IRequestHandler<GetResumenPorEstadoReporteQuery, Result<IReadOnlyList<ResumenPorEstadoDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetResumenPorEstadoReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<ResumenPorEstadoDto>>> Handle(
        GetResumenPorEstadoReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.ResumenPorEstado);
        if (!auth.IsSuccess) return Result<IReadOnlyList<ResumenPorEstadoDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var filtros = ReporteAuthorization.ApplyScope(_currentUser, ToFiltros(request.Filtros));
        var items = await _reportes.GetResumenPorEstadoAsync(filtros, cancellationToken);
        return Result<IReadOnlyList<ResumenPorEstadoDto>>.Success(items);
    }

    internal static ReporteFiltros ToFiltros(ReporteFiltrosRequest request) =>
        new(request.Desde, request.Hasta, request.EmpleadoId, request.JefeId, request.Departamento, request.Estado);
}

public record GetLegalizacionesDetalleReporteQuery(ReporteFiltrosRequest Filtros)
    : IRequest<Result<IReadOnlyList<LegalizacionDetalleReporteDto>>>;

public class GetLegalizacionesDetalleReporteQueryHandler
    : IRequestHandler<GetLegalizacionesDetalleReporteQuery, Result<IReadOnlyList<LegalizacionDetalleReporteDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetLegalizacionesDetalleReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<LegalizacionDetalleReporteDto>>> Handle(
        GetLegalizacionesDetalleReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.LegalizacionesDetalle);
        if (!auth.IsSuccess)
            return Result<IReadOnlyList<LegalizacionDetalleReporteDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var filtros = ReporteAuthorization.ApplyScope(_currentUser, GetResumenPorEstadoReporteQueryHandler.ToFiltros(request.Filtros));
        var items = await _reportes.GetLegalizacionesDetalleAsync(filtros, cancellationToken);
        return Result<IReadOnlyList<LegalizacionDetalleReporteDto>>.Success(items);
    }
}

public record GetGastosPorCategoriaReporteQuery(ReporteFiltrosRequest Filtros)
    : IRequest<Result<IReadOnlyList<GastoPorCategoriaDto>>>;

public class GetGastosPorCategoriaReporteQueryHandler
    : IRequestHandler<GetGastosPorCategoriaReporteQuery, Result<IReadOnlyList<GastoPorCategoriaDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetGastosPorCategoriaReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<GastoPorCategoriaDto>>> Handle(
        GetGastosPorCategoriaReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.GastosPorCategoria);
        if (!auth.IsSuccess) return Result<IReadOnlyList<GastoPorCategoriaDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var filtros = ReporteAuthorization.ApplyScope(_currentUser, GetResumenPorEstadoReporteQueryHandler.ToFiltros(request.Filtros));
        var items = await _reportes.GetGastosPorCategoriaAsync(filtros, cancellationToken);
        return Result<IReadOnlyList<GastoPorCategoriaDto>>.Success(items);
    }
}

public record GetGastosDetalleReporteQuery(ReporteFiltrosRequest Filtros)
    : IRequest<Result<IReadOnlyList<GastoDetalleReporteDto>>>;

public class GetGastosDetalleReporteQueryHandler
    : IRequestHandler<GetGastosDetalleReporteQuery, Result<IReadOnlyList<GastoDetalleReporteDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetGastosDetalleReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<GastoDetalleReporteDto>>> Handle(
        GetGastosDetalleReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.GastosDetalle);
        if (!auth.IsSuccess) return Result<IReadOnlyList<GastoDetalleReporteDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var filtros = ReporteAuthorization.ApplyScope(_currentUser, GetResumenPorEstadoReporteQueryHandler.ToFiltros(request.Filtros));
        var items = await _reportes.GetGastosDetalleAsync(filtros, cancellationToken);
        return Result<IReadOnlyList<GastoDetalleReporteDto>>.Success(items);
    }
}

public record GetResumenFinancieroEmpleadoReporteQuery(ReporteFiltrosRequest Filtros)
    : IRequest<Result<IReadOnlyList<ResumenFinancieroEmpleadoDto>>>;

public class GetResumenFinancieroEmpleadoReporteQueryHandler
    : IRequestHandler<GetResumenFinancieroEmpleadoReporteQuery, Result<IReadOnlyList<ResumenFinancieroEmpleadoDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetResumenFinancieroEmpleadoReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<ResumenFinancieroEmpleadoDto>>> Handle(
        GetResumenFinancieroEmpleadoReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.ResumenFinancieroEmpleado);
        if (!auth.IsSuccess)
            return Result<IReadOnlyList<ResumenFinancieroEmpleadoDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var filtros = ReporteAuthorization.ApplyScope(_currentUser, GetResumenPorEstadoReporteQueryHandler.ToFiltros(request.Filtros));
        var items = await _reportes.GetResumenFinancieroEmpleadoAsync(
            filtros,
            request.Filtros.SoloCerradas,
            cancellationToken);
        return Result<IReadOnlyList<ResumenFinancieroEmpleadoDto>>.Success(items);
    }
}

public record GetPendientesAprobacionReporteQuery : IRequest<Result<IReadOnlyList<PendienteAprobacionReporteDto>>>;

public class GetPendientesAprobacionReporteQueryHandler
    : IRequestHandler<GetPendientesAprobacionReporteQuery, Result<IReadOnlyList<PendienteAprobacionReporteDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetPendientesAprobacionReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<PendienteAprobacionReporteDto>>> Handle(
        GetPendientesAprobacionReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.PendientesAprobacion);
        if (!auth.IsSuccess)
            return Result<IReadOnlyList<PendienteAprobacionReporteDto>>.Failure(auth.ErrorCode!, auth.Error!);

        Guid? jefeId = _currentUser.IsInRole("Admin") ? null : _currentUser.UserId;

        var items = await _reportes.GetPendientesAprobacionAsync(jefeId, cancellationToken);
        return Result<IReadOnlyList<PendienteAprobacionReporteDto>>.Success(items);
    }
}

public record GetPendientesNominaReporteQuery : IRequest<Result<IReadOnlyList<PendienteNominaReporteDto>>>;

public class GetPendientesNominaReporteQueryHandler
    : IRequestHandler<GetPendientesNominaReporteQuery, Result<IReadOnlyList<PendienteNominaReporteDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetPendientesNominaReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<PendienteNominaReporteDto>>> Handle(
        GetPendientesNominaReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.PendientesNomina);
        if (!auth.IsSuccess)
            return Result<IReadOnlyList<PendienteNominaReporteDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var items = await _reportes.GetPendientesNominaAsync(cancellationToken);
        return Result<IReadOnlyList<PendienteNominaReporteDto>>.Success(items);
    }
}

public record GetLegalizacionesCerradasReporteQuery(ReporteFiltrosRequest Filtros)
    : IRequest<Result<IReadOnlyList<LegalizacionCerradaReporteDto>>>;

public class GetLegalizacionesCerradasReporteQueryHandler
    : IRequestHandler<GetLegalizacionesCerradasReporteQuery, Result<IReadOnlyList<LegalizacionCerradaReporteDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetLegalizacionesCerradasReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<LegalizacionCerradaReporteDto>>> Handle(
        GetLegalizacionesCerradasReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.LegalizacionesCerradas);
        if (!auth.IsSuccess)
            return Result<IReadOnlyList<LegalizacionCerradaReporteDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var filtros = ReporteAuthorization.ApplyScope(_currentUser, GetResumenPorEstadoReporteQueryHandler.ToFiltros(request.Filtros));
        var items = await _reportes.GetLegalizacionesCerradasAsync(filtros, cancellationToken);
        return Result<IReadOnlyList<LegalizacionCerradaReporteDto>>.Success(items);
    }
}

public record GetGastosSinSoporteReporteQuery(ReporteFiltrosRequest Filtros)
    : IRequest<Result<IReadOnlyList<GastoSinSoporteDto>>>;

public class GetGastosSinSoporteReporteQueryHandler
    : IRequestHandler<GetGastosSinSoporteReporteQuery, Result<IReadOnlyList<GastoSinSoporteDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetGastosSinSoporteReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<GastoSinSoporteDto>>> Handle(
        GetGastosSinSoporteReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.GastosSinSoporte);
        if (!auth.IsSuccess) return Result<IReadOnlyList<GastoSinSoporteDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var filtros = ReporteAuthorization.ApplyScope(_currentUser, GetResumenPorEstadoReporteQueryHandler.ToFiltros(request.Filtros));
        var items = await _reportes.GetGastosSinSoporteAsync(filtros, cancellationToken);
        return Result<IReadOnlyList<GastoSinSoporteDto>>.Success(items);
    }
}

public record GetHistorialAuditoriaReporteQuery(ReporteFiltrosRequest Filtros, Guid? LegalizacionId = null)
    : IRequest<Result<IReadOnlyList<HistorialAuditoriaDto>>>;

public class GetHistorialAuditoriaReporteQueryHandler
    : IRequestHandler<GetHistorialAuditoriaReporteQuery, Result<IReadOnlyList<HistorialAuditoriaDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetHistorialAuditoriaReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<HistorialAuditoriaDto>>> Handle(
        GetHistorialAuditoriaReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.HistorialAuditoria);
        if (!auth.IsSuccess) return Result<IReadOnlyList<HistorialAuditoriaDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var filtros = ReporteAuthorization.ApplyScope(_currentUser, GetResumenPorEstadoReporteQueryHandler.ToFiltros(request.Filtros));
        var items = await _reportes.GetHistorialAuditoriaAsync(filtros, request.LegalizacionId, cancellationToken);
        return Result<IReadOnlyList<HistorialAuditoriaDto>>.Success(items);
    }
}

public record GetVolumenMensualReporteQuery(ReporteFiltrosRequest Filtros)
    : IRequest<Result<IReadOnlyList<VolumenMensualDto>>>;

public class GetVolumenMensualReporteQueryHandler
    : IRequestHandler<GetVolumenMensualReporteQuery, Result<IReadOnlyList<VolumenMensualDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetVolumenMensualReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<VolumenMensualDto>>> Handle(
        GetVolumenMensualReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.VolumenMensual);
        if (!auth.IsSuccess) return Result<IReadOnlyList<VolumenMensualDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var filtros = ReporteAuthorization.ApplyScope(_currentUser, GetResumenPorEstadoReporteQueryHandler.ToFiltros(request.Filtros));
        var items = await _reportes.GetVolumenMensualAsync(filtros, request.Filtros.Anio, cancellationToken);
        return Result<IReadOnlyList<VolumenMensualDto>>.Success(items);
    }
}

public record GetTiemposPorEstadoReporteQuery(ReporteFiltrosRequest Filtros)
    : IRequest<Result<IReadOnlyList<TiempoPorEstadoDto>>>;

public class GetTiemposPorEstadoReporteQueryHandler
    : IRequestHandler<GetTiemposPorEstadoReporteQuery, Result<IReadOnlyList<TiempoPorEstadoDto>>>
{
    private readonly IReporteRepository _reportes;
    private readonly ICurrentUserService _currentUser;

    public GetTiemposPorEstadoReporteQueryHandler(IReporteRepository reportes, ICurrentUserService currentUser)
    {
        _reportes = reportes;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<TiempoPorEstadoDto>>> Handle(
        GetTiemposPorEstadoReporteQuery request,
        CancellationToken cancellationToken)
    {
        var auth = ReporteAuthorization.EnsureCanAccess(_currentUser, ReporteTipo.TiemposPorEstado);
        if (!auth.IsSuccess) return Result<IReadOnlyList<TiempoPorEstadoDto>>.Failure(auth.ErrorCode!, auth.Error!);

        var filtros = ReporteAuthorization.ApplyScope(_currentUser, GetResumenPorEstadoReporteQueryHandler.ToFiltros(request.Filtros));
        var items = await _reportes.GetTiemposPorEstadoAsync(filtros, cancellationToken);
        return Result<IReadOnlyList<TiempoPorEstadoDto>>.Success(items);
    }
}
