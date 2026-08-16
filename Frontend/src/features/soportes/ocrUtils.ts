import type { EstadoOcr, OcrCampo, OcrCampoFormValue } from "@/types/soporte";

const estadoLabels: Record<EstadoOcr, string> = {
  Pendiente: "Pendiente",
  Procesando: "Procesando",
  Completado: "Completado",
  Error: "Error",
  ValidadoUsuario: "Validado",
};

const estadoTone: Record<EstadoOcr, "neutral" | "info" | "success" | "warning" | "danger"> = {
  Pendiente: "neutral",
  Procesando: "info",
  Completado: "success",
  Error: "danger",
  ValidadoUsuario: "success",
};

const fieldLabels: Record<string, string> = {
  proveedor: "Proveedor",
  numero_documento: "Número de documento",
  monto: "Monto",
  fecha_gasto: "Fecha del gasto",
};

export function getOcrEstadoLabel(estado: EstadoOcr | null | undefined): string {
  if (!estado) return "Sin OCR";
  return estadoLabels[estado] ?? estado;
}

export function getOcrEstadoTone(
  estado: EstadoOcr | null | undefined,
): "neutral" | "info" | "success" | "warning" | "danger" {
  if (!estado) return "neutral";
  return estadoTone[estado] ?? "neutral";
}

export function getOcrCampoLabel(nombreCampo: string): string {
  return fieldLabels[nombreCampo] ?? nombreCampo;
}

export function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

export function camposToFormValues(campos: OcrCampo[]): OcrCampoFormValue[] {
  return campos.map((campo) => ({
    campoId: campo.id,
    nombreCampo: campo.nombreCampo,
    value: campo.valorValidado ?? campo.valorExtraido ?? "",
  }));
}

export function canProcesarOcr(estado: EstadoOcr | null | undefined): boolean {
  return estado === "Pendiente" || estado === "Error";
}

export function canEditOcrCampos(estado: EstadoOcr | null | undefined): boolean {
  return estado === "Completado" || estado === "ValidadoUsuario";
}

export function canAplicarOcr(estado: EstadoOcr | null | undefined): boolean {
  return estado === "Completado" || estado === "ValidadoUsuario";
}

export const ACCEPTED_SOPORTE_TYPES = ".jpg,.jpeg,.png,.pdf";
export const ACCEPTED_SOPORTE_MIME = ["image/jpeg", "image/png", "application/pdf"];
