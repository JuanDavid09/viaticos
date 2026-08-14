import { useCallback, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Plus } from "lucide-react";
import { Topbar } from "@/components/layout/Topbar";
import { LegalizacionList } from "@/components/legalizaciones/LegalizacionList";
import { listMisLegalizaciones } from "@/api/legalizaciones";
import { appRoutes } from "@/app/routes";
import { ApiError } from "@/types/auth";
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
      setError(err instanceof ApiError ? err.message : "No se pudo cargar el listado.");
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

        {error ? <p className="login-error" role="alert">{error}</p> : null}
        {isLoading ? <p>Cargando legalizaciones…</p> : <LegalizacionList items={items} />}
      </main>
    </>
  );
}
