import { apiFormRequest, apiRequest } from "@/api/http";
import type { LegalizacionDetalle } from "@/types/legalizacion";
import type {
  OcrExtraccion,
  SubirSoporteResponse,
  ValidarCamposOcrRequest,
} from "@/types/soporte";

export async function subirSoporte(
  legalizacionId: string,
  gastoId: string,
  file: File,
  esPrincipal = false,
): Promise<SubirSoporteResponse> {
  const formData = new FormData();
  formData.append("legalizacionId", legalizacionId);
  formData.append("gastoId", gastoId);
  formData.append("file", file);
  formData.append("esPrincipal", String(esPrincipal));

  return apiFormRequest<SubirSoporteResponse>("/api/soportes", formData);
}

export async function getOcrExtraccion(gastoSoporteId: string): Promise<OcrExtraccion> {
  return apiRequest<OcrExtraccion>(`/api/soportes/${gastoSoporteId}/ocr`);
}

export async function procesarOcr(gastoSoporteId: string): Promise<OcrExtraccion> {
  return apiRequest<OcrExtraccion>(`/api/soportes/${gastoSoporteId}/ocr/procesar`, {
    method: "POST",
  });
}

export async function validarCamposOcr(
  gastoSoporteId: string,
  request: ValidarCamposOcrRequest,
): Promise<OcrExtraccion> {
  return apiRequest<OcrExtraccion>(`/api/soportes/${gastoSoporteId}/ocr/campos`, {
    method: "PUT",
    body: request,
  });
}

export async function aplicarOcrAGasto(gastoSoporteId: string): Promise<LegalizacionDetalle> {
  return apiRequest<LegalizacionDetalle>(`/api/soportes/${gastoSoporteId}/ocr/aplicar`, {
    method: "POST",
  });
}
