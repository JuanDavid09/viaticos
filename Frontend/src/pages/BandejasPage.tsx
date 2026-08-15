import { useCallback, useEffect, useMemo, useState } from "react";
import { Topbar } from "@/components/layout/Topbar";
import { LegalizacionList } from "@/components/legalizaciones/LegalizacionList";
import { listPendientesAprobacion, listPendientesNomina } from "@/api/bandejas";
import { useAuth } from "@/features/auth/AuthContext";
import { ApiError } from "@/types/auth";
import type { LegalizacionResumen } from "@/types/legalizacion";

type BandejaTab = "aprobacion" | "nomina";

export function BandejasPage() {
  const { hasRole } = useAuth();
  const canViewAprobacion = hasRole("JEFE_APROBADOR", "ADMIN");
  const canViewNomina = hasRole("NOMINA", "ADMIN");

  const defaultTab = useMemo<BandejaTab>(() => {
    if (canViewAprobacion) return "aprobacion";
    return "nomina";
  }, [canViewAprobacion]);

  const [activeTab, setActiveTab] = useState<BandejaTab>(defaultTab);
  const [aprobacionItems, setAprobacionItems] = useState<LegalizacionResumen[]>([]);
  const [nominaItems, setNominaItems] = useState<LegalizacionResumen[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadData = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [aprobacion, nomina] = await Promise.all([
        canViewAprobacion ? listPendientesAprobacion() : Promise.resolve([]),
        canViewNomina ? listPendientesNomina() : Promise.resolve([]),
      ]);
      setAprobacionItems(aprobacion);
      setNominaItems(nomina);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudieron cargar las bandejas.");
    } finally {
      setIsLoading(false);
    }
  }, [canViewAprobacion, canViewNomina]);

  useEffect(() => {
    void loadData();
  }, [loadData]);

  const showTabs = canViewAprobacion && canViewNomina;
  const currentItems = activeTab === "aprobacion" ? aprobacionItems : nominaItems;

  return (
    <>
      <Topbar title="Bandejas" kicker="Aprobación" />
      <main className="content">
        <p className="page-lead">
          Revisa las legalizaciones pendientes de tu rol y abre el detalle para aprobar, rechazar o cerrar.
        </p>

        {showTabs ? (
          <div className="tab-bar" role="tablist" aria-label="Bandejas">
            <button
              type="button"
              role="tab"
              aria-selected={activeTab === "aprobacion"}
              className={`tab-button${activeTab === "aprobacion" ? " active" : ""}`}
              onClick={() => setActiveTab("aprobacion")}
            >
              Pendientes de aprobación ({aprobacionItems.length})
            </button>
            <button
              type="button"
              role="tab"
              aria-selected={activeTab === "nomina"}
              className={`tab-button${activeTab === "nomina" ? " active" : ""}`}
              onClick={() => setActiveTab("nomina")}
            >
              Pendientes de nómina ({nominaItems.length})
            </button>
          </div>
        ) : null}

        {error ? <p className="login-error" role="alert">{error}</p> : null}
        {isLoading ? <p>Cargando bandejas…</p> : null}

        {!isLoading ? (
          <section className="card">
            <LegalizacionList
              items={currentItems}
              fromBandejas
              emptyTitle={
                activeTab === "aprobacion"
                  ? "No hay pendientes de aprobación"
                  : "No hay pendientes de nómina"
              }
              emptyDescription={
                activeTab === "aprobacion"
                  ? "Cuando un empleado envíe una legalización, aparecerá aquí para tu revisión."
                  : "Las legalizaciones aprobadas y enviadas a nómina se listarán aquí."
              }
            />
          </section>
        ) : null}
      </main>
    </>
  );
}
