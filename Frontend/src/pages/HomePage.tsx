import { useCallback, useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import {
  AlertCircle,
  CheckCircle2,
  Clock3,
  FileText,
  Inbox,
  Plus,
  ShieldCheck,
} from "lucide-react";
import { Topbar } from "@/components/layout/Topbar";
import { DashboardStatCard } from "@/components/dashboard/DashboardStatCard";
import { PendingActionsList } from "@/components/dashboard/PendingActionsList";
import { RecentLegalizaciones } from "@/components/dashboard/RecentLegalizaciones";
import { ViaticosCalendar } from "@/components/dashboard/ViaticosCalendar";
import { ErrorBanner } from "@/components/ui/ErrorBanner";
import { LoadingState } from "@/components/ui/LoadingState";
import { listPendientesAprobacion, listPendientesNomina } from "@/api/bandejas";
import { listMisLegalizaciones } from "@/api/legalizaciones";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";
import { buildEmployeeDashboard } from "@/features/dashboard/employeeDashboard";
import { getApiErrorMessage } from "@/lib/apiErrorMessage";

export function HomePage() {
  const { session, hasRole } = useAuth();
  const [legalizaciones, setLegalizaciones] = useState<Awaited<ReturnType<typeof listMisLegalizaciones>>>([]);
  const [pendientesAprobacion, setPendientesAprobacion] = useState(0);
  const [pendientesNomina, setPendientesNomina] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const showEmployeeDashboard = hasRole("EMPLEADO", "JEFE_APROBADOR", "ADMIN");
  const showBandejaJefe = hasRole("JEFE_APROBADOR", "ADMIN");
  const showBandejaNomina = hasRole("NOMINA", "ADMIN");
  const showTeamCalendar = hasRole("JEFE_APROBADOR", "ADMIN");
  const calendarScopeLabel = hasRole("ADMIN") ? "equipo y organización" : "equipo";

  const loadData = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    const tasks: Promise<{ ok: true } | { ok: false; message: string }>[] = [];

    if (showEmployeeDashboard) {
      tasks.push(
        listMisLegalizaciones()
          .then((data) => {
            setLegalizaciones(data);
            return { ok: true as const };
          })
          .catch((err) => ({ ok: false as const, message: getApiErrorMessage(err) })),
      );
    }

    if (showBandejaJefe) {
      tasks.push(
        listPendientesAprobacion()
          .then((data) => {
            setPendientesAprobacion(data.length);
            return { ok: true as const };
          })
          .catch((err) => ({ ok: false as const, message: getApiErrorMessage(err) })),
      );
    }

    if (showBandejaNomina) {
      tasks.push(
        listPendientesNomina()
          .then((data) => {
            setPendientesNomina(data.length);
            return { ok: true as const };
          })
          .catch((err) => ({ ok: false as const, message: getApiErrorMessage(err) })),
      );
    }

    const results = await Promise.all(tasks);
    const failures = results.filter((result): result is { ok: false; message: string } => !result.ok);

    if (failures.length > 0) {
      setError(failures[0].message);
    }

    setIsLoading(false);
  }, [showEmployeeDashboard, showBandejaJefe, showBandejaNomina]);

  useEffect(() => {
    void loadData();
  }, [loadData]);

  const employeeDashboard = useMemo(
    () => buildEmployeeDashboard(legalizaciones),
    [legalizaciones],
  );

  const greeting = session
    ? `Hola, ${session.nombreCompleto.split(" ")[0]}`
    : "Bienvenido";

  return (
    <>
      <Topbar title="Inicio" kicker="Panel principal" />
      <main className="content">
        <section className="dashboard-hero">
          <div>
            <h1 className="dashboard-greeting">{greeting}</h1>
            <p className="page-lead dashboard-lead">
              {showEmployeeDashboard
                ? "Aquí tienes un resumen de tus legalizaciones, pendientes y reportes recientes."
                : showTeamCalendar
                  ? "Consulta el calendario de viáticos del equipo y el estado de tus tareas."
                  : "Consulta el estado de tus tareas y accede rápidamente a los módulos disponibles."}
            </p>
          </div>

          {showEmployeeDashboard ? (
            <Link className="btn btn-primary" to={`${appRoutes.legalizaciones}/nueva`}>
              <Plus size={16} />
              Nueva legalización
            </Link>
          ) : null}
        </section>

        {error ? <ErrorBanner message={error} onRetry={() => void loadData()} /> : null}

        {isLoading ? <LoadingState label="Cargando panel…" skeletonRows={3} /> : null}

        {!isLoading ? (
          <>
            {showEmployeeDashboard ? (
              <>
                <section className="dashboard-stats">
                  <DashboardStatCard
                    label="Total reportes"
                    value={employeeDashboard.stats.total}
                    icon={<FileText size={18} />}
                  />
                  <DashboardStatCard
                    label="Borradores"
                    value={employeeDashboard.stats.borradores}
                    tone="neutral"
                    icon={<Clock3 size={18} />}
                  />
                  <DashboardStatCard
                    label="En proceso"
                    value={employeeDashboard.stats.enProceso}
                    tone="info"
                    icon={<ShieldCheck size={18} />}
                  />
                  <DashboardStatCard
                    label="Rechazadas"
                    value={employeeDashboard.stats.rechazadas}
                    tone="danger"
                    icon={<AlertCircle size={18} />}
                  />
                  <DashboardStatCard
                    label="Cerradas"
                    value={employeeDashboard.stats.cerradas}
                    tone="success"
                    icon={<CheckCircle2 size={18} />}
                  />
                </section>

                <section className="grid grid-2 dashboard-panels">
                  <article className="card">
                    <div className="dashboard-section-header">
                      <h3>Pendientes de acción</h3>
                      <span className="dashboard-count">{employeeDashboard.pendientes.length}</span>
                    </div>
                    <PendingActionsList items={employeeDashboard.pendientes} />
                  </article>

                  <article className="card">
                    <div className="dashboard-section-header">
                      <h3>Reportes recientes</h3>
                      <Link className="dashboard-link" to={appRoutes.legalizaciones}>
                        Ver todas
                      </Link>
                    </div>
                    <RecentLegalizaciones items={employeeDashboard.recientes} />
                  </article>
                </section>
              </>
            ) : null}

            {(showBandejaJefe || showBandejaNomina) && !showEmployeeDashboard ? (
              <section className="dashboard-stats">
                {showBandejaJefe ? (
                  <DashboardStatCard
                    label="Pendientes de aprobación"
                    value={pendientesAprobacion}
                    tone="info"
                    icon={<Inbox size={18} />}
                  />
                ) : null}
                {showBandejaNomina ? (
                  <DashboardStatCard
                    label="Pendientes de nómina"
                    value={pendientesNomina}
                    tone="warning"
                    icon={<Inbox size={18} />}
                  />
                ) : null}
              </section>
            ) : null}

            {(showBandejaJefe || showBandejaNomina) ? (
              <section className="grid grid-2 dashboard-panels">
                {showBandejaJefe ? (
                  <article className="card dashboard-quick-card">
                    <h3>Bandeja de aprobación</h3>
                    <p className="table-meta">
                      Tienes {pendientesAprobacion} legalización
                      {pendientesAprobacion === 1 ? "" : "es"} pendiente
                      {pendientesAprobacion === 1 ? "" : "s"} de revisión.
                    </p>
                    <Link className="btn btn-primary" to={appRoutes.bandejas}>
                      Ir a bandejas
                    </Link>
                  </article>
                ) : null}

                {showBandejaNomina ? (
                  <article className="card dashboard-quick-card">
                    <h3>Bandeja de nómina</h3>
                    <p className="table-meta">
                      Tienes {pendientesNomina} legalización
                      {pendientesNomina === 1 ? "" : "es"} pendiente
                      {pendientesNomina === 1 ? "" : "s"} de cierre.
                    </p>
                    <Link className="btn btn-primary" to={appRoutes.bandejas}>
                      Ir a bandejas
                    </Link>
                  </article>
                ) : null}
              </section>
            ) : null}

            {showTeamCalendar ? (
              <section className="dashboard-calendar-section">
                <ViaticosCalendar scopeLabel={calendarScopeLabel} />
              </section>
            ) : null}
          </>
        ) : null}
      </main>
    </>
  );
}
