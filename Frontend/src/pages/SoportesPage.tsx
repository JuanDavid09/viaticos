import { useCallback, useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { FileUp } from "lucide-react";
import { Topbar } from "@/components/layout/Topbar";
import { LegalizacionList } from "@/components/legalizaciones/LegalizacionList";
import { ErrorBanner } from "@/components/ui/ErrorBanner";
import { LoadingState } from "@/components/ui/LoadingState";
import { listMisLegalizaciones } from "@/api/legalizaciones";
import { appRoutes } from "@/app/routes";
import { isEditable } from "@/features/legalizaciones/legalizacionUtils";
import { getApiErrorMessage } from "@/lib/apiErrorMessage";
import type { LegalizacionResumen } from "@/types/legalizacion";

export function SoportesPage() {
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

  const editableItems = useMemo(
    () => items.filter((item) => isEditable(item.estado)),
    [items],
  );

  return (
    <>
      <Topbar title="Soportes y OCR" kicker="Documentos" />
      <main className="content">
        <div className="phase-banner">
          <div>
            <strong>Adjunta facturas y aplica datos al gasto</strong>
            <p className="page-lead" style={{ marginBottom: 0 }}>
              Abre una legalización editable, sube JPG/PNG/PDF y usa el OCR para completar el gasto.
            </p>
          </div>
          <FileUp size={22} />
        </div>

        {error ? <ErrorBanner message={error} onRetry={() => void loadData()} /> : null}
        {isLoading ? <LoadingState label="Cargando legalizaciones…" skeletonRows={3} /> : null}

        {!isLoading ? (
          <section className="card">
            <h3>Legalizaciones editables ({editableItems.length})</h3>
            <p className="table-meta" style={{ marginBottom: 16 }}>
              También puedes gestionar soportes desde el detalle de cada legalización.
            </p>
            <LegalizacionList
              items={editableItems}
              emptyTitle="No hay legalizaciones editables"
              emptyDescription="Crea una legalización en borrador y agrega gastos para adjuntar soportes."
            />
          </section>
        ) : null}
      </main>
    </>
  );
}
