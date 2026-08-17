import {
  formatDate,
  formatDateTime,
  formatMoney,
  getEstadoLabel,
} from "@/features/legalizaciones/legalizacionUtils";
import type { ReportColumn } from "@/features/reportes/reportDefinitions";
import type { EstadoLegalizacion } from "@/types/legalizacion";

export function formatReportCellValue(
  value: unknown,
  format?: ReportColumn["format"],
): string {
  if (value === null || value === undefined || value === "") return "";

  switch (format) {
    case "money":
      return formatMoney(Number(value));
    case "date":
      return formatDate(String(value));
    case "datetime":
      return formatDateTime(String(value));
    case "number":
      return new Intl.NumberFormat("es-CO").format(Number(value));
    case "estado":
      return getEstadoLabel(String(value) as EstadoLegalizacion);
    default:
      if (typeof value === "boolean") return value ? "Sí" : "No";
      return String(value);
  }
}

export function getReportExportValue(
  row: Record<string, unknown>,
  column: ReportColumn,
): string | number | boolean {
  const value = row[column.key];
  if (value === null || value === undefined || value === "") return "";

  if (column.format === "number" || column.format === "money") {
    const numeric = Number(value);
    return Number.isFinite(numeric) ? numeric : formatReportCellValue(value, column.format);
  }

  return formatReportCellValue(value, column.format);
}

export function buildReportMatrix(
  columns: ReportColumn[],
  rows: Record<string, unknown>[],
): (string | number | boolean)[][] {
  const header = columns.map((column) => column.label);
  const body = rows.map((row) =>
    columns.map((column) => getReportExportValue(row, column)),
  );
  return [header, ...body];
}

export function buildReportFilename(reportLabel: string): string {
  const slug = reportLabel
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "");

  const date = new Date().toISOString().slice(0, 10);
  return `reporte-${slug}-${date}`;
}
