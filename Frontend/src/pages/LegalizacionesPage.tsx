import { useCallback, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Plus } from "lucide-react";
import { Topbar } from "@/components/layout/Topbar";
import { LegalizacionList } from "@/components/legalizaciones/LegalizacionList";
import { ErrorBanner } from "@/components/ui/ErrorBanner";
import { LoadingState } from "@/components/ui/LoadingState";
import { listMisLegalizaciones } from "@/api/legalizaciones";
import { appRoutes } from "@/app/routes";
import { getApiErrorMessage } from "@/lib/apiErrorMessage";
import type { LegalizacionResumen } from "@/types/legalizacion";

export function LegalizacionesPage() {
  const [items, setItems] = useState<LegalizacionResumen[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadData = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const data = await listMisLegalizaciones();
      setItems(data);
    } catch (err) {
      setError(getApiErrorMessage(err, "No se pudo cargar el listado."));
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadData();
  }, [loadData]);

  return (
    <>
      <Topbar title="Mis legalizaciones" kicker="Empleado" />
      <main className="content">
        <div className="page-toolbar">
          <p className="page-lead" style={{ marginBottom: 0 }}>
            Registra tus viajes, agrega gastos en borrador y prepáralos para el flujo de aprobación.
          </p>
          <Link className="btn btn-primary" to={`${appRoutes.legalizaciones}/nueva`}>
            <Plus size={16} />
            Nueva legalización
          </Link>
        </div>

        {error ? <ErrorBanner message={error} onRetry={() => void loadData()} /> : null}
        {isLoading ? (
          <LoadingState label="Cargando legalizaciones…" skeletonRows={4} />
        ) : (
          <LegalizacionList items={items} />
        )}
      </main>
    </>
  );
}
