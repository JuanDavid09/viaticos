import { apiRequest } from "@/api/http";
import type { Empleado } from "@/types/empleado";
import type {
  ActualizarLegalizacionRequest,
  AgregarGastoRequest,
  CrearLegalizacionRequest,
  LegalizacionCalendario,
  LegalizacionDetalle,
  LegalizacionHistorial,
  LegalizacionResumen,
  RechazarLegalizacionRequest,
} from "@/types/legalizacion";

export async function listMisLegalizaciones(): Promise<LegalizacionResumen[]> {
  return apiRequest<LegalizacionResumen[]>("/api/legalizaciones");
}

export async function listEmpleadosAsignables(): Promise<Empleado[]> {
  return apiRequest<Empleado[]>("/api/legalizaciones/empleados-asignables");
}

export async function listCalendarioLegalizaciones(params?: {
  desde?: string;
  hasta?: string;
}): Promise<LegalizacionCalendario[]> {
  const search = new URLSearchParams();
  if (params?.desde) search.set("desde", params.desde);
  if (params?.hasta) search.set("hasta", params.hasta);
  const query = search.toString();
  return apiRequest<LegalizacionCalendario[]>(
    `/api/legalizaciones/calendario${query ? `?${query}` : ""}`,
  );
}

export async function getLegalizacion(id: string): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/legalizaciones/${id}`);
}

export async function createLegalizacion(
  request: CrearLegalizacionRequest,
): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>("/api/legalizaciones", {
    method: "POST",
    body: request,
  });
}

export async function updateLegalizacion(
  id: string,
  request: ActualizarLegalizacionRequest,
): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/legalizaciones/${id}`, {
    method: "PUT",
    body: request,
  });
}

export async function addGasto(
  legalizacionId: string,
  request: AgregarGastoRequest,
): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/legalizaciones/${legalizacionId}/gastos`, {
    method: "POST",
    body: request,
  });
}

export async function getHistorial(legalizacionId: string): Promise<LegalizacionHistorial[]> {
  return apiRequest<LegalizacionHistorial[]>(`/api/legalizaciones/${legalizacionId}/historial`);
}

export async function enviarValidacion(id: string): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/legalizaciones/${id}/enviar-validacion`, {
    method: "POST",
  });
}

export async function enviarAprobacion(id: string): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/legalizaciones/${id}/enviar-aprobacion`, {
    method: "POST",
  });
}

export async function aprobarLegalizacion(id: string): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/legalizaciones/${id}/aprobar`, {
    method: "POST",
  });
}

export async function rechazarLegalizacion(
  id: string,
  request: RechazarLegalizacionRequest,
): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/legalizaciones/${id}/rechazar`, {
    method: "POST",
    body: request,
  });
}

export async function reabrirLegalizacion(id: string): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/legalizaciones/${id}/reabrir`, {
    method: "POST",
  });
}

export async function enviarNomina(id: string): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/legalizaciones/${id}/enviar-nomina`, {
    method: "POST",
  });
}

export async function cerrarLegalizacion(id: string): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/legalizaciones/${id}/cerrar`, {
    method: "POST",
  });
}
