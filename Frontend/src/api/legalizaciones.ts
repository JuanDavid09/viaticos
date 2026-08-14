import { apiRequest } from "@/api/http";
import type {
  ActualizarLegalizacionRequest,
  AgregarGastoRequest,
  CrearLegalizacionRequest,
  LegalizacionDetalle,
  LegalizacionResumen,
} from "@/types/legalizacion";

export async function listMisLegalizaciones(): Promise<LegalizacionResumen[]> {
  return apiRequest<LegalizacionResumen[]>("/api/legalizaciones");
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
