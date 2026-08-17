import { apiRequest } from "@/api/http";
import type {
  GastoDetalleReporte,
  GastoPorCategoria,
  GastoSinSoporte,
  HistorialAuditoria,
  LegalizacionCerradaReporte,
  LegalizacionDetalleReporte,
  PendienteAprobacionReporte,
  PendienteNominaReporte,
  ReporteFiltros,
  ReporteTipo,
  ResumenFinancieroEmpleado,
  ResumenPorEstado,
  TiempoPorEstado,
  VolumenMensual,
} from "@/types/reportes";

function buildQuery(filtros?: ReporteFiltros): string {
  if (!filtros) return "";

  const search = new URLSearchParams();
  if (filtros.desde) search.set("desde", filtros.desde);
  if (filtros.hasta) search.set("hasta", filtros.hasta);
  if (filtros.departamento) search.set("departamento", filtros.departamento);
  if (filtros.estado) search.set("estado", filtros.estado);
  if (filtros.anio) search.set("anio", String(filtros.anio));
  if (filtros.soloCerradas !== undefined) search.set("soloCerradas", String(filtros.soloCerradas));

  const query = search.toString();
  return query ? `?${query}` : "";
}

export async function runReporte<T>(
  tipo: ReporteTipo,
  filtros?: ReporteFiltros,
): Promise<T[]> {
  const query = buildQuery(filtros);

  switch (tipo) {
    case "resumen-por-estado":
      return apiRequest<T[]>(`/api/reportes/resumen-por-estado${query}`);
    case "legalizaciones-detalle":
      return apiRequest<T[]>(`/api/reportes/legalizaciones-detalle${query}`);
    case "gastos-por-categoria":
      return apiRequest<T[]>(`/api/reportes/gastos-por-categoria${query}`);
    case "gastos-detalle":
      return apiRequest<T[]>(`/api/reportes/gastos-detalle${query}`);
    case "resumen-financiero-empleado":
      return apiRequest<T[]>(`/api/reportes/resumen-financiero-empleado${query}`);
    case "pendientes-aprobacion":
      return apiRequest<T[]>(`/api/reportes/pendientes-aprobacion`);
    case "pendientes-nomina":
      return apiRequest<T[]>(`/api/reportes/pendientes-nomina`);
    case "legalizaciones-cerradas":
      return apiRequest<T[]>(`/api/reportes/legalizaciones-cerradas${query}`);
    case "gastos-sin-soporte":
      return apiRequest<T[]>(`/api/reportes/gastos-sin-soporte${query}`);
    case "historial-auditoria":
      return apiRequest<T[]>(`/api/reportes/historial-auditoria${query}`);
    case "volumen-mensual":
      return apiRequest<T[]>(`/api/reportes/volumen-mensual${query}`);
    case "tiempos-por-estado":
      return apiRequest<T[]>(`/api/reportes/tiempos-por-estado${query}`);
    default:
      throw new Error(`Reporte no soportado: ${tipo satisfies never}`);
  }
}

export type ReporteResultMap = {
  "resumen-por-estado": ResumenPorEstado;
  "legalizaciones-detalle": LegalizacionDetalleReporte;
  "gastos-por-categoria": GastoPorCategoria;
  "gastos-detalle": GastoDetalleReporte;
  "resumen-financiero-empleado": ResumenFinancieroEmpleado;
  "pendientes-aprobacion": PendienteAprobacionReporte;
  "pendientes-nomina": PendienteNominaReporte;
  "legalizaciones-cerradas": LegalizacionCerradaReporte;
  "gastos-sin-soporte": GastoSinSoporte;
  "historial-auditoria": HistorialAuditoria;
  "volumen-mensual": VolumenMensual;
  "tiempos-por-estado": TiempoPorEstado;
};

export async function fetchReporte<T extends ReporteTipo>(
  tipo: T,
  filtros?: ReporteFiltros,
): Promise<ReporteResultMap[T][]> {
  return runReporte<ReporteResultMap[T]>(tipo, filtros);
}
