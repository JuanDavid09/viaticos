import { useCallback, useEffect, useState, type FormEvent } from "react";
import { Link, useLocation, useParams } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import { Topbar } from "@/components/layout/Topbar";
import { EstadoBadge } from "@/components/legalizaciones/EstadoBadge";
import { GastoForm } from "@/components/legalizaciones/GastoForm";
import { HistorialTimeline } from "@/components/legalizaciones/HistorialTimeline";
import { LegalizacionForm } from "@/components/legalizaciones/LegalizacionForm";
import { WorkflowActions } from "@/components/legalizaciones/WorkflowActions";
import {
  addGasto,
  aprobarLegalizacion,
  cerrarLegalizacion,
  enviarAprobacion,
  enviarNomina,
  enviarValidacion,
  getHistorial,
  getLegalizacion,
  reabrirLegalizacion,
  rechazarLegalizacion,
  updateLegalizacion,
} from "@/api/legalizaciones";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";
import {
  findCategoria,
  findMoneda,
  useCatalogos,
} from "@/features/legalizaciones/useCatalogos";
import {
  formatDate,
  formatMoney,
  isEditable,
  legalizacionToFormValues,
  parseGastoRequest,
  parseLegalizacionRequest,
} from "@/features/legalizaciones/legalizacionUtils";
import { getWorkflowActionLabel } from "@/features/legalizaciones/workflowUtils";
import { ApiError } from "@/types/auth";
import {
  emptyGastoForm,
  type GastoFormValues,
  type LegalizacionDetalle,
  type LegalizacionFormValues,
  type LegalizacionHistorial,
  type WorkflowAction,
} from "@/types/legalizacion";

export function LegalizacionDetallePage() {
  const { id } = useParams<{ id: string }>();
  const location = useLocation();
  const { session, hasRole } = useAuth();
  const fromBandejas =
    (location.state as { fromBandejas?: boolean } | null)?.fromBandejas ?? false;
  const backTo = fromBandejas ? appRoutes.bandejas : appRoutes.legalizaciones;
  const backLabel = fromBandejas ? "Volver a bandejas" : "Volver al listado";

  const { catalogos, isLoading: isLoadingCatalogos, error: catalogosError } = useCatalogos();
  const [legalizacion, setLegalizacion] = useState<LegalizacionDetalle | null>(null);
  const [historial, setHistorial] = useState<LegalizacionHistorial[]>([]);
  const [form, setForm] = useState<LegalizacionFormValues | null>(null);
  const [gastoForm, setGastoForm] = useState<GastoFormValues>(emptyGastoForm);
  const [isLoading, setIsLoading] = useState(true);
  const [isLoadingHistorial, setIsLoadingHistorial] = useState(true);
  const [isSavingLegalizacion, setIsSavingLegalizacion] = useState(false);
  const [isSavingGasto, setIsSavingGasto] = useState(false);
  const [isSubmittingWorkflow, setIsSubmittingWorkflow] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const loadHistorial = useCallback(async () => {
    if (!id) return;
    setIsLoadingHistorial(true);
    try {
      const data = await getHistorial(id);
      setHistorial(data);
    } catch {
      setHistorial([]);
    } finally {
      setIsLoadingHistorial(false);
    }
  }, [id]);

  const loadData = useCallback(async () => {
    if (!id) return;
    setIsLoading(true);
    setError(null);
    try {
      const data = await getLegalizacion(id);
      setLegalizacion(data);
      setForm(legalizacionToFormValues(data));
      setGastoForm((current) => ({
        ...emptyGastoForm,
        fechaGasto: current.fechaGasto || data.fechaInicio.slice(0, 10),
      }));
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo cargar la legalización.");
    } finally {
      setIsLoading(false);
    }
  }, [id]);

  useEffect(() => {
    void loadData();
    void loadHistorial();
  }, [loadData, loadHistorial]);

  async function handleSaveLegalizacion(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!id || !form) return;

    setIsSavingLegalizacion(true);
    setError(null);
    setSuccess(null);

    try {
      const updated = await updateLegalizacion(id, parseLegalizacionRequest(form));
      setLegalizacion(updated);
      setForm(legalizacionToFormValues(updated));
      setSuccess("Legalización actualizada.");
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo guardar la legalización.");
    } finally {
      setIsSavingLegalizacion(false);
    }
  }

  async function handleAddGasto(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!id) return;

    setIsSavingGasto(true);
    setError(null);
    setSuccess(null);

    try {
      const updated = await addGasto(id, parseGastoRequest(gastoForm));
      setLegalizacion(updated);
      setGastoForm({
        ...emptyGastoForm,
        fechaGasto: updated.fechaInicio.slice(0, 10),
      });
      setSuccess("Gasto agregado.");
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo agregar el gasto.");
    } finally {
      setIsSavingGasto(false);
    }
  }

  async function handleWorkflowAction(action: WorkflowAction, comentario?: string) {
    if (!id || !session) return;

    setIsSubmittingWorkflow(true);
    setError(null);
    setSuccess(null);

    try {
      let updated: LegalizacionDetalle;
      switch (action) {
        case "enviar-validacion":
          updated = await enviarValidacion(id);
          break;
        case "enviar-aprobacion":
          updated = await enviarAprobacion(id);
          break;
        case "aprobar":
          updated = await aprobarLegalizacion(id);
          break;
        case "rechazar":
          updated = await rechazarLegalizacion(id, { comentario: comentario ?? "" });
          break;
        case "reabrir":
          updated = await reabrirLegalizacion(id);
          break;
        case "enviar-nomina":
          updated = await enviarNomina(id);
          break;
        case "cerrar":
          updated = await cerrarLegalizacion(id);
          break;
      }

      setLegalizacion(updated);
      setForm(legalizacionToFormValues(updated));
      setSuccess(`${getWorkflowActionLabel(action)} completado.`);
      await loadHistorial();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo ejecutar la acción.");
    } finally {
      setIsSubmittingWorkflow(false);
    }
  }

  const moneda = legalizacion ? findMoneda(catalogos, legalizacion.monedaId) : null;
  const editable =
    legalizacion && session
      ? isEditable(legalizacion.estado) &&
        (legalizacion.empleadoId === session.userId || hasRole("ADMIN"))
      : false;
  const canEditGastos = editable && legalizacion?.estado === "Borrador";

  return (
    <>
      <Topbar
        title={legalizacion?.numero ?? "Detalle"}
        kicker="Legalización"
      />
      <main className="content">
        <Link className="back-link" to={backTo}>
          <ArrowLeft size={16} />
          {backLabel}
        </Link>

        {isLoading ? <p>Cargando detalle…</p> : null}
        {catalogosError ? <p className="login-error" role="alert">{catalogosError}</p> : null}
        {error ? <p className="login-error" role="alert">{error}</p> : null}
        {success ? <p className="success-banner">{success}</p> : null}

        {legalizacion && form && session ? (
          <>
            <section className="detail-header card">
              <div className="detail-header-main">
                <div className="row-title">
                  <h3 style={{ margin: 0 }}>{legalizacion.motivo}</h3>
                  <EstadoBadge estado={legalizacion.estado} />
                </div>
                <p className="table-meta">
                  {formatDate(legalizacion.fechaInicio)} → {formatDate(legalizacion.fechaFin)}
                  {legalizacion.destino ? ` · ${legalizacion.destino}` : ""}
                </p>
              </div>
              <div className="totals-grid">
                <div>
                  <span className="total-label">Anticipo</span>
                  <strong>{formatMoney(legalizacion.montoAnticipo, moneda)}</strong>
                </div>
                <div>
                  <span className="total-label">Total gastos</span>
                  <strong>{formatMoney(legalizacion.totalGastos, moneda)}</strong>
                </div>
                <div>
                  <span className="total-label">Reembolso</span>
                  <strong>{formatMoney(legalizacion.totalReembolso, moneda)}</strong>
                </div>
                <div>
                  <span className="total-label">Devolución</span>
                  <strong>{formatMoney(legalizacion.totalDevolucion, moneda)}</strong>
                </div>
              </div>
            </section>

            <WorkflowActions
              legalizacion={legalizacion}
              rol={session.rol}
              userId={session.userId}
              isSubmitting={isSubmittingWorkflow}
              onAction={handleWorkflowAction}
            />

            <section className="grid grid-2 admin-layout">
              <article className="card card-form">
                <h3>Datos del viaje</h3>
                {editable ? (
                  isLoadingCatalogos ? (
                    <p>Cargando catálogos…</p>
                  ) : (
                    <LegalizacionForm
                      form={form}
                      catalogos={catalogos}
                      isSubmitting={isSavingLegalizacion}
                      submitLabel="Guardar cambios"
                      onChange={setForm}
                      onSubmit={handleSaveLegalizacion}
                    />
                  )
                ) : (
                  <dl className="detail-list">
                    <div><dt>Motivo</dt><dd>{legalizacion.motivo}</dd></div>
                    <div><dt>Destino</dt><dd>{legalizacion.destino || "—"}</dd></div>
                    <div><dt>Fechas</dt><dd>{formatDate(legalizacion.fechaInicio)} → {formatDate(legalizacion.fechaFin)}</dd></div>
                    <div><dt>Moneda</dt><dd>{moneda ? `${moneda.codigoIso} — ${moneda.nombre}` : "—"}</dd></div>
                    <div><dt>Anticipo</dt><dd>{formatMoney(legalizacion.montoAnticipo, moneda)}</dd></div>
                  </dl>
                )}
              </article>

              <article className="card card-form">
                <h3>Gastos ({legalizacion.gastos.length})</h3>

                {legalizacion.gastos.length === 0 ? (
                  <p className="table-meta">Aún no hay gastos registrados.</p>
                ) : (
                  <div className="table-list gastos-list">
                    {legalizacion.gastos.map((gasto) => {
                      const categoria = findCategoria(catalogos, gasto.categoriaGastoId);
                      return (
                        <div key={gasto.id} className="table-row">
                          <div>
                            <strong>{gasto.descripcion}</strong>
                            <span className="table-meta">
                              {categoria?.nombre ?? "Categoría"} · {formatDate(gasto.fechaGasto)}
                            </span>
                            {gasto.proveedor ? (
                              <span className="table-meta">{gasto.proveedor}</span>
                            ) : null}
                          </div>
                          <strong>{formatMoney(gasto.monto, moneda)}</strong>
                        </div>
                      );
                    })}
                  </div>
                )}

                {canEditGastos ? (
                  <div className="section-divider">
                    <h4>Agregar gasto</h4>
                    {isLoadingCatalogos ? (
                      <p>Cargando catálogos…</p>
                    ) : (
                      <GastoForm
                        form={gastoForm}
                        catalogos={catalogos}
                        isSubmitting={isSavingGasto}
                        onChange={setGastoForm}
                        onSubmit={handleAddGasto}
                      />
                    )}
                  </div>
                ) : null}
              </article>
            </section>

            <section className="card">
              <h3>Historial de estados</h3>
              {isLoadingHistorial ? (
                <p>Cargando historial…</p>
              ) : (
                <HistorialTimeline items={historial} />
              )}
            </section>
          </>
        ) : null}
      </main>
    </>
  );
}
