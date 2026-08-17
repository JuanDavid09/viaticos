import { useCallback, useEffect, useMemo, useState } from "react";
import { Topbar } from "@/components/layout/Topbar";
import { ReporteExportActions } from "@/components/reportes/ReporteExportActions";
import { ReporteFilters } from "@/components/reportes/ReporteFilters";
import { ReporteTable } from "@/components/reportes/ReporteTable";
import { ErrorBanner } from "@/components/ui/ErrorBanner";
import { LoadingState } from "@/components/ui/LoadingState";
import { fetchReporte } from "@/api/reportes";
import { useAuth } from "@/features/auth/AuthContext";
import {
  getReportDefinitionsForRole,
  reportColumns,
} from "@/features/reportes/reportDefinitions";
import { getApiErrorMessage } from "@/lib/apiErrorMessage";
import type { ReporteFiltros, ReporteTipo } from "@/types/reportes";

function getDefaultFiltros(): ReporteFiltros {
  const today = new Date();
  const year = today.getFullYear();
  const month = String(today.getMonth() + 1).padStart(2, "0");
  const day = String(today.getDate()).padStart(2, "0");
  const firstDay = `${year}-${month}-01`;
  const todayIso = `${year}-${month}-${day}`;

  return {
    desde: firstDay,
    hasta: todayIso,
    anio: year,
    soloCerradas: true,
  };
}

export function ReportesPage() {
  const { session, hasRole } = useAuth();
  const availableReports = useMemo(
    () => (session ? getReportDefinitionsForRole(session.rol) : []),
    [session],
  );

  const [selectedReport, setSelectedReport] = useState<ReporteTipo>(
    availableReports[0]?.id ?? "resumen-por-estado",
  );
  const [filtros, setFiltros] = useState<ReporteFiltros>(() => getDefaultFiltros());
  const [rows, setRows] = useState<Record<string, unknown>[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [hasGenerated, setHasGenerated] = useState(false);

  const currentDefinition = useMemo(
    () => availableReports.find((item) => item.id === selectedReport) ?? availableReports[0],
    [availableReports, selectedReport],
  );

  useEffect(() => {
    if (availableReports.length === 0) return;
    if (!availableReports.some((item) => item.id === selectedReport)) {
      setSelectedReport(availableReports[0].id);
    }
  }, [availableReports, selectedReport]);

  const loadReport = useCallback(async () => {
    if (!currentDefinition) return;

    setIsLoading(true);
    setError(null);

    try {
      const data = await fetchReporte(currentDefinition.id, filtros);
      setRows(data as Record<string, unknown>[]);
      setHasGenerated(true);
    } catch (err) {
      setError(getApiErrorMessage(err, "No se pudo generar el reporte."));
    } finally {
      setIsLoading(false);
    }
  }, [currentDefinition, filtros]);

  const kicker = hasRole("ADMIN")
    ? "Administración"
    : hasRole("NOMINA")
      ? "Nómina"
      : "Supervisión";

  if (!currentDefinition) {
    return (
      <>
        <Topbar title="Reportes" kicker={kicker} />
        <main className="content">
          <p className="page-lead">No tienes reportes disponibles para tu rol.</p>
        </main>
      </>
    );
  }

  return (
    <>
      <Topbar title="Reportes" kicker={kicker} />
      <main className="content">
        <p className="page-lead">
          Consulta reportes operativos y financieros generados desde los procedimientos almacenados
          de la base de datos.
        </p>

        <section className="card report-selector-card">
          <label htmlFor="reporte-tipo">
            Tipo de reporte
            <select
              id="reporte-tipo"
              value={selectedReport}
              onChange={(event) => {
                setSelectedReport(event.target.value as ReporteTipo);
                setRows([]);
                setHasGenerated(false);
                setError(null);
              }}
              disabled={isLoading}
            >
              {availableReports.map((report) => (
                <option key={report.id} value={report.id}>
                  {report.label}
                </option>
              ))}
            </select>
          </label>
          <p className="table-meta">{currentDefinition.description}</p>
        </section>

        <section className="card card-form">
          <h3>Filtros</h3>
          <ReporteFilters
            definition={currentDefinition}
            filtros={filtros}
            isLoading={isLoading}
            onChange={setFiltros}
            onSubmit={() => void loadReport()}
          />
        </section>

        {error ? <ErrorBanner message={error} onRetry={() => void loadReport()} /> : null}

        <section className="card">
          <div className="dashboard-section-header report-results-header">
            <div className="report-results-title">
              <h3>Resultados</h3>
              {hasGenerated ? <span className="dashboard-count">{rows.length}</span> : null}
            </div>
            {hasGenerated ? (
              <ReporteExportActions
                reportLabel={currentDefinition.label}
                columns={reportColumns[currentDefinition.id]}
                rows={rows}
                disabled={isLoading}
              />
            ) : null}
          </div>

          {isLoading ? <LoadingState label="Generando reporte…" skeletonRows={5} /> : null}

          {!isLoading && hasGenerated ? (
            <ReporteTable
              columns={reportColumns[currentDefinition.id]}
              rows={rows}
              emptyTitle="El reporte no devolvió registros"
            />
          ) : null}

          {!isLoading && !hasGenerated ? (
            <p className="table-meta">Selecciona filtros y pulsa “Generar reporte”.</p>
          ) : null}
        </section>
      </main>
    </>
  );
}
