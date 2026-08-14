import { useCallback, useEffect, useState, type FormEvent } from "react";
import { Link, useParams } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import { Topbar } from "@/components/layout/Topbar";
import { EstadoBadge } from "@/components/legalizaciones/EstadoBadge";
import { GastoForm } from "@/components/legalizaciones/GastoForm";
import { LegalizacionForm } from "@/components/legalizaciones/LegalizacionForm";
import { addGasto, getLegalizacion, updateLegalizacion } from "@/api/legalizaciones";
import { appRoutes } from "@/app/routes";
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
import { ApiError } from "@/types/auth";
import {
  emptyGastoForm,
  type GastoFormValues,
  type LegalizacionDetalle,
  type LegalizacionFormValues,
} from "@/types/legalizacion";

export function LegalizacionDetallePage() {
  const { id } = useParams<{ id: string }>();
  const { catalogos, isLoading: isLoadingCatalogos, error: catalogosError } = useCatalogos();
  const [legalizacion, setLegalizacion] = useState<LegalizacionDetalle | null>(null);
  const [form, setForm] = useState<LegalizacionFormValues | null>(null);
  const [gastoForm, setGastoForm] = useState<GastoFormValues>(emptyGastoForm);
  const [isLoading, setIsLoading] = useState(true);
  const [isSavingLegalizacion, setIsSavingLegalizacion] = useState(false);
  const [isSavingGasto, setIsSavingGasto] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

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
  }, [loadData]);

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

  const moneda = legalizacion ? findMoneda(catalogos, legalizacion.monedaId) : null;
  const editable = legalizacion ? isEditable(legalizacion.estado) : false;

  return (
    <>
      <Topbar
        title={legalizacion?.numero ?? "Detalle"}
        kicker="Legalización"
      />
      <main className="content">
        <Link className="back-link" to={appRoutes.legalizaciones}>
          <ArrowLeft size={16} />
          Volver al listado
        </Link>

        {isLoading ? <p>Cargando detalle…</p> : null}
        {catalogosError ? <p className="login-error" role="alert">{catalogosError}</p> : null}
        {error ? <p className="login-error" role="alert">{error}</p> : null}
        {success ? <p className="success-banner">{success}</p> : null}

        {legalizacion && form ? (
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

                {editable ? (
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
          </>
        ) : null}
      </main>
    </>
  );
}
