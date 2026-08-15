import { apiRequest } from "@/api/http";
import type { LegalizacionResumen } from "@/types/legalizacion";

export async function listPendientesAprobacion(): Promise<LegalizacionResumen[]> {
  return apiRequest<LegalizacionResumen[]>("/api/bandejas/pendientes-aprobacion");
}

export async function listPendientesNomina(): Promise<LegalizacionResumen[]> {
  return apiRequest<LegalizacionResumen[]>("/api/bandejas/pendientes-nomina");
}
