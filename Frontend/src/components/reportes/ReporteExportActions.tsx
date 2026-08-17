import { FileSpreadsheet, FileText } from "lucide-react";
import {
  buildReportFilename,
  downloadReportCsv,
  downloadReportExcel,
} from "@/features/reportes/exportReport";
import type { ReportColumn } from "@/features/reportes/reportDefinitions";

type ReporteExportActionsProps = {
  reportLabel: string;
  columns: ReportColumn[];
  rows: Record<string, unknown>[];
  disabled?: boolean;
};

export function ReporteExportActions({
  reportLabel,
  columns,
  rows,
  disabled = false,
}: ReporteExportActionsProps) {
  const canExport = !disabled && rows.length > 0;
  const filename = buildReportFilename(reportLabel);

  function handleExportCsv() {
    if (!canExport) return;
    downloadReportCsv(columns, rows, filename);
  }

  async function handleExportExcel() {
    if (!canExport) return;
    await downloadReportExcel(columns, rows, filename, reportLabel);
  }

  return (
    <div className="report-export-actions">
      <button
        type="button"
        className="btn btn-ghost"
        onClick={handleExportCsv}
        disabled={!canExport}
      >
        <FileText size={16} />
        Exportar CSV
      </button>
      <button
        type="button"
        className="btn btn-ghost"
        onClick={() => void handleExportExcel()}
        disabled={!canExport}
      >
        <FileSpreadsheet size={16} />
        Exportar Excel
      </button>
    </div>
  );
}
