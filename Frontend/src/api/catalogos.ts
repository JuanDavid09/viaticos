import { apiRequest } from "@/api/http";
import type { Catalogos } from "@/types/catalogos";

export async function getCatalogos(): Promise<Catalogos> {
  return apiRequest<Catalogos>("/api/catalogos");
}
