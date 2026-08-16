import { useEffect, useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import { Topbar } from "@/components/layout/Topbar";
import { LegalizacionForm } from "@/components/legalizaciones/LegalizacionForm";
import { ErrorBanner } from "@/components/ui/ErrorBanner";
import { LoadingState } from "@/components/ui/LoadingState";
import { createLegalizacion } from "@/api/legalizaciones";
import { appRoutes } from "@/app/routes";
import { useCatalogos } from "@/features/legalizaciones/useCatalogos";
import { parseLegalizacionRequest } from "@/features/legalizaciones/legalizacionUtils";
import { getApiErrorMessage } from "@/lib/apiErrorMessage";
import { emptyLegalizacionForm, type LegalizacionFormValues } from "@/types/legalizacion";

export function LegalizacionNuevaPage() {
  const navigate = useNavigate();
  const { catalogos, isLoading: isLoadingCatalogos, error: catalogosError, reload } = useCatalogos();
  const [form, setForm] = useState<LegalizacionFormValues>(() => {
    const initial = { ...emptyLegalizacionForm };
    const today = new Date().toISOString().slice(0, 10);
    initial.fechaInicio = today;
    initial.fechaFin = today;
    return initial;
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!catalogos || form.monedaId) return;
    const cop = catalogos.monedas.find((m) => m.codigoIso === "COP") ?? catalogos.monedas[0];
    if (cop) setForm((current) => ({ ...current, monedaId: cop.id }));
  }, [catalogos, form.monedaId]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      const created = await createLegalizacion(parseLegalizacionRequest(form));
      navigate(`${appRoutes.legalizaciones}/${created.id}`, { replace: true });
    } catch (err) {
      setError(getApiErrorMessage(err, "No se pudo crear la legalización."));
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <>
      <Topbar title="Nueva legalización" kicker="Empleado" />
      <main className="content">
        <Link className="back-link" to={appRoutes.legalizaciones}>
          <ArrowLeft size={16} />
          Volver al listado
        </Link>

        <p className="page-lead">
          Completa los datos del viaje. Podrás agregar gastos mientras la legalización esté en borrador.
        </p>

        {catalogosError ? (
          <ErrorBanner message={catalogosError} onRetry={() => void reload()} />
        ) : null}
        {error ? <ErrorBanner message={error} /> : null}

        <article className="card card-form">
          <h3>Datos del viaje</h3>
          {isLoadingCatalogos ? (
            <LoadingState label="Cargando catálogos…" />
          ) : (
            <LegalizacionForm
              form={form}
              catalogos={catalogos}
              isSubmitting={isSubmitting}
              submitLabel="Crear legalización"
              onChange={setForm}
              onSubmit={handleSubmit}
            />
          )}
        </article>
      </main>
    </>
  );
}
