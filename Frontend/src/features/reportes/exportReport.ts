import type { ReportColumn } from "@/features/reportes/reportDefinitions";
import {
  buildReportFilename,
  buildReportMatrix,
} from "@/features/reportes/reportFormatUtils";

function triggerDownload(blob: Blob, filename: string) {
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement("a");
  anchor.href = url;
  anchor.download = filename;
  anchor.click();
  URL.revokeObjectURL(url);
}

function escapeCsvCell(value: string | number | boolean): string {
  const text = String(value);
  if (/[",\n\r]/.test(text)) {
    return `"${text.replace(/"/g, '""')}"`;
  }
  return text;
}

export { buildReportFilename };

export function downloadReportCsv(
  columns: ReportColumn[],
  rows: Record<string, unknown>[],
  filename: string,
) {
  const matrix = buildReportMatrix(columns, rows);
  const content = matrix
    .map((line) => line.map((cell) => escapeCsvCell(cell)).join(","))
    .join("\r\n");

  const blob = new Blob(["\uFEFF", content], { type: "text/csv;charset=utf-8;" });
  triggerDownload(blob, `${filename}.csv`);
}

export async function downloadReportExcel(
  columns: ReportColumn[],
  rows: Record<string, unknown>[],
  filename: string,
  sheetName = "Reporte",
) {
  const XLSX = await import("xlsx");
  const matrix = buildReportMatrix(columns, rows);
  const worksheet = XLSX.utils.aoa_to_sheet(matrix);
  worksheet["!cols"] = columns.map((column) => ({
    wch: Math.max(column.label.length + 2, column.format === "money" ? 16 : 14),
  }));

  const workbook = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(workbook, worksheet, sheetName.slice(0, 31));
  XLSX.writeFile(workbook, `${filename}.xlsx`);
}
