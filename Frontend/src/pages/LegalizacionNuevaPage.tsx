import { useEffect, useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import { Topbar } from "@/components/layout/Topbar";
import { LegalizacionForm } from "@/components/legalizaciones/LegalizacionForm";
import { ErrorBanner } from "@/components/ui/ErrorBanner";
import { LoadingState } from "@/components/ui/LoadingState";
import { createLegalizacion, listEmpleadosAsignables } from "@/api/legalizaciones";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";
import { useCatalogos } from "@/features/legalizaciones/useCatalogos";
import { parseLegalizacionRequest } from "@/features/legalizaciones/legalizacionUtils";
import { getApiErrorMessage } from "@/lib/apiErrorMessage";
import type { Empleado } from "@/types/empleado";
import { emptyLegalizacionForm, type LegalizacionFormValues } from "@/types/legalizacion";

export function LegalizacionNuevaPage() {
  const navigate = useNavigate();
  const { hasRole } = useAuth();
  const canAssignEmpleado = hasRole("JEFE_APROBADOR", "ADMIN");
  const { catalogos, isLoading: isLoadingCatalogos, error: catalogosError, reload } = useCatalogos();
  const [empleadosAsignables, setEmpleadosAsignables] = useState<Empleado[] | null>(null);
  const [isLoadingEmpleados, setIsLoadingEmpleados] = useState(canAssignEmpleado);
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
    if (!canAssignEmpleado) {
      setIsLoadingEmpleados(false);
      return;
    }

    let cancelled = false;

    async function loadEmpleados() {
      setIsLoadingEmpleados(true);
      try {
        const data = await listEmpleadosAsignables();
        if (!cancelled) setEmpleadosAsignables(data);
      } catch (err) {
        if (!cancelled) {
          setError(getApiErrorMessage(err, "No se pudo cargar la lista de empleados."));
        }
      } finally {
        if (!cancelled) setIsLoadingEmpleados(false);
      }
    }

    void loadEmpleados();

    return () => {
      cancelled = true;
    };
  }, [canAssignEmpleado]);

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

  const kicker = canAssignEmpleado ? "Supervisión" : "Empleado";
  const pageLead = canAssignEmpleado
    ? "Registra tu propio viaje o crea una legalización en borrador para un empleado de tu equipo."
    : "Completa los datos del viaje. Podrás agregar gastos mientras la legalización esté en borrador.";

  const submitLabel = form.empleadoId
    ? "Crear para empleado"
    : "Crear legalización";

  const isLoadingForm = isLoadingCatalogos || (canAssignEmpleado && isLoadingEmpleados);

  return (
    <>
      <Topbar title="Nueva legalización" kicker={kicker} />
      <main className="content legalizacion-nueva-page">
        <Link className="back-link" to={appRoutes.legalizaciones}>
          <ArrowLeft size={16} />
          Volver al listado
        </Link>

        <p className="page-lead">{pageLead}</p>

        {catalogosError ? (
          <ErrorBanner message={catalogosError} onRetry={() => void reload()} />
        ) : null}
        {error ? <ErrorBanner message={error} /> : null}

        <article className="card card-form">
          <h3>Datos del viaje</h3>
          {isLoadingForm ? (
            <LoadingState label="Cargando formulario…" />
          ) : (
            <LegalizacionForm
              form={form}
              catalogos={catalogos}
              allowEmpleadoAsignacion={canAssignEmpleado}
              empleadosAsignables={empleadosAsignables}
              isSubmitting={isSubmitting}
              submitLabel={submitLabel}
              onChange={setForm}
              onSubmit={handleSubmit}
            />
          )}
        </article>
      </main>
    </>
  );
}
