import type { EstadoLegalizacion } from "@/types/legalizacion";
import type { Moneda } from "@/types/catalogos";

const estadoLabels: Record<EstadoLegalizacion, string> = {
  Borrador: "Borrador",
  PendienteValidacion: "Pendiente validación",
  PendienteAprobacion: "Pendiente aprobación",
  Aprobada: "Aprobada",
  Rechazada: "Rechazada",
  PendienteNomina: "Pendiente nómina",
  Cerrada: "Cerrada",
};

const estadoTone: Record<EstadoLegalizacion, "neutral" | "info" | "success" | "warning" | "danger"> = {
  Borrador: "neutral",
  PendienteValidacion: "info",
  PendienteAprobacion: "info",
  Aprobada: "success",
  Rechazada: "danger",
  PendienteNomina: "warning",
  Cerrada: "success",
};

export function getEstadoLabel(estado: EstadoLegalizacion): string {
  return estadoLabels[estado] ?? estado;
}

export function getEstadoTone(estado: EstadoLegalizacion): string {
  return estadoTone[estado] ?? "neutral";
}

export function isEditable(estado: EstadoLegalizacion): boolean {
  return estado === "Borrador" || estado === "PendienteValidacion";
}

export function formatMoney(amount: number, moneda?: Moneda | null): string {
  const symbol = moneda?.simbolo?.trim() || moneda?.codigoIso || "";
  return formatMoneyWithSymbol(amount, symbol);
}

export function formatMoneyWithSymbol(amount: number, symbol?: string | null): string {
  const trimmed = symbol?.trim() || "";
  const formatted = new Intl.NumberFormat("es-CO", {
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  }).format(amount);

  return trimmed ? `${trimmed} ${formatted}` : formatted;
}

export function formatDate(value: string): string {
  if (!value) return "—";
  const [year, month, day] = value.split("-");
  if (!year || !month || !day) return value;
  return `${day}/${month}/${year}`;
}

export function formatDateTime(value: string): string {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return new Intl.DateTimeFormat("es-CO", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(date);
}

export function toApiDateInput(value: string): string {
  return value.slice(0, 10);
}

export function legalizacionToFormValues(
  legalizacion: Pick<
    import("@/types/legalizacion").LegalizacionDetalle,
    "motivo" | "destino" | "fechaInicio" | "fechaFin" | "monedaId" | "montoAnticipo"
  >,
): import("@/types/legalizacion").LegalizacionFormValues {
  return {
    motivo: legalizacion.motivo,
    destino: legalizacion.destino ?? "",
    fechaInicio: toApiDateInput(legalizacion.fechaInicio),
    fechaFin: toApiDateInput(legalizacion.fechaFin),
    monedaId: legalizacion.monedaId,
    montoAnticipo: String(legalizacion.montoAnticipo),
  };
}

export function parseLegalizacionRequest(
  form: import("@/types/legalizacion").LegalizacionFormValues,
): import("@/types/legalizacion").CrearLegalizacionRequest {
  return {
    motivo: form.motivo.trim(),
    destino: form.destino.trim() || undefined,
    fechaInicio: form.fechaInicio,
    fechaFin: form.fechaFin,
    monedaId: form.monedaId,
    montoAnticipo: Number(form.montoAnticipo) || 0,
  };
}

export function parseGastoRequest(
  form: import("@/types/legalizacion").GastoFormValues,
): import("@/types/legalizacion").AgregarGastoRequest {
  return {
    categoriaGastoId: form.categoriaGastoId,
    fechaGasto: form.fechaGasto,
    descripcion: form.descripcion.trim(),
    monto: Number(form.monto),
    proveedor: form.proveedor.trim() || undefined,
    numeroDocumento: form.numeroDocumento.trim() || undefined,
  };
}
