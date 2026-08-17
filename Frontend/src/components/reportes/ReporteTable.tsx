import { EstadoBadge } from "@/components/legalizaciones/EstadoBadge";
import { EmptyState } from "@/components/ui/EmptyState";
import { formatReportCellValue } from "@/features/reportes/reportFormatUtils";
import type { ReportColumn } from "@/features/reportes/reportDefinitions";
import type { EstadoLegalizacion } from "@/types/legalizacion";

type ReporteTableProps = {
  columns: ReportColumn[];
  rows: Record<string, unknown>[];
  emptyTitle?: string;
};

export function ReporteTable({
  columns,
  rows,
  emptyTitle = "Sin resultados",
}: ReporteTableProps) {
  if (rows.length === 0) {
    return (
      <EmptyState
        title={emptyTitle}
        description="Ajusta los filtros o selecciona otro reporte."
      />
    );
  }

  return (
    <div className="report-table-wrap">
      <table className="report-table">
        <thead>
          <tr>
            {columns.map((column) => (
              <th key={column.key}>{column.label}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={index}>
              {columns.map((column) => {
                const value = row[column.key];

                if (column.format === "estado" && value) {
                  return (
                    <td key={column.key}>
                      <EstadoBadge estado={String(value) as EstadoLegalizacion} />
                    </td>
                  );
                }

                const display = formatReportCellValue(value, column.format);
                return (
                  <td key={column.key}>{display || "—"}</td>
                );
              })}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
