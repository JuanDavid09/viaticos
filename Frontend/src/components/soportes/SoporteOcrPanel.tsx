import { useEffect, useState, type FormEvent } from "react";
import { CheckCircle2, RefreshCw, Sparkles } from "lucide-react";
import { OcrEstadoBadge } from "@/components/soportes/OcrEstadoBadge";
import {
  aplicarOcrAGasto,
  getOcrExtraccion,
  procesarOcr,
  validarCamposOcr,
} from "@/api/soportes";
import {
  camposToFormValues,
  canAplicarOcr,
  canEditOcrCampos,
  canProcesarOcr,
  getOcrCampoLabel,
} from "@/features/soportes/ocrUtils";
import { ApiError } from "@/types/auth";
import type { LegalizacionDetalle } from "@/types/legalizacion";
import type { GastoSoporte, OcrCampoFormValue, OcrExtraccion } from "@/types/soporte";

type SoporteOcrPanelProps = {
  soporte: GastoSoporte;
  editable: boolean;
  onLegalizacionUpdated: (legalizacion: LegalizacionDetalle) => void;
  onMessage: (message: string | null) => void;
  onError: (message: string | null) => void;
};

export function SoporteOcrPanel({
  soporte,
  editable,
  onLegalizacionUpdated,
  onMessage,
  onError,
}: SoporteOcrPanelProps) {
  const [ocr, setOcr] = useState<OcrExtraccion | null>(null);
  const [camposForm, setCamposForm] = useState<OcrCampoFormValue[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    let cancelled = false;

    async function loadOcr() {
      if (!soporte.ocrExtraccionId) return;
      setIsLoading(true);
      onError(null);
      try {
        const data = await getOcrExtraccion(soporte.id);
        if (!cancelled) {
          setOcr(data);
          setCamposForm(camposToFormValues(data.campos));
        }
      } catch (err) {
        if (!cancelled) {
          onError(err instanceof ApiError ? err.message : "No se pudo cargar el OCR.");
        }
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    }

    void loadOcr();
    return () => {
      cancelled = true;
    };
  }, [soporte.id, soporte.ocrExtraccionId, onError]);

  async function handleProcesar() {
    setIsSubmitting(true);
    onError(null);
    onMessage(null);
    try {
      const data = await procesarOcr(soporte.id);
      setOcr(data);
      setCamposForm(camposToFormValues(data.campos));
      onMessage("OCR procesado correctamente.");
    } catch (err) {
      onError(err instanceof ApiError ? err.message : "No se pudo procesar el OCR.");
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleValidar(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSubmitting(true);
    onError(null);
    onMessage(null);
    try {
      const data = await validarCamposOcr(soporte.id, {
        campos: camposForm.map((campo) => ({
          campoId: campo.campoId,
          valorValidado: campo.value.trim(),
        })),
      });
      setOcr(data);
      setCamposForm(camposToFormValues(data.campos));
      onMessage("Campos OCR validados.");
    } catch (err) {
      onError(err instanceof ApiError ? err.message : "No se pudieron validar los campos.");
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleAplicar() {
    setIsSubmitting(true);
    onError(null);
    onMessage(null);
    try {
      const updated = await aplicarOcrAGasto(soporte.id);
      onLegalizacionUpdated(updated);
      onMessage("Datos OCR aplicados al gasto.");
    } catch (err) {
      onError(err instanceof ApiError ? err.message : "No se pudieron aplicar los datos al gasto.");
    } finally {
      setIsSubmitting(false);
    }
  }

  const estado = ocr?.estado ?? soporte.ocrEstado;

  return (
    <div className="soporte-ocr-panel">
      <div className="soporte-item-header">
        <div>
          <strong>{soporte.nombreOriginal}</strong>
          {soporte.esPrincipal ? <span className="badge">Principal</span> : null}
        </div>
        <OcrEstadoBadge estado={estado} />
      </div>

      {ocr?.errorMensaje ? (
        <p className="login-error" role="alert">{ocr.errorMensaje}</p>
      ) : null}

      {isLoading ? <p className="table-meta">Cargando OCR…</p> : null}

      {editable && canProcesarOcr(estado) ? (
        <button
          type="button"
          className="btn btn-ghost"
          disabled={isSubmitting}
          onClick={() => void handleProcesar()}
        >
          <Sparkles size={16} />
          {isSubmitting ? "Procesando…" : "Procesar OCR"}
        </button>
      ) : null}

      {!isLoading && ocr && canEditOcrCampos(estado) ? (
        <form className="stack-form ocr-campos-form" onSubmit={(event) => void handleValidar(event)}>
          {camposForm.map((campo, index) => (
            <label key={campo.campoId} htmlFor={`ocr-${campo.campoId}`}>
              {getOcrCampoLabel(campo.nombreCampo)}
              <input
                id={`ocr-${campo.campoId}`}
                type={campo.nombreCampo === "fecha_gasto" ? "date" : "text"}
                value={campo.nombreCampo === "fecha_gasto" ? campo.value.slice(0, 10) : campo.value}
                onChange={(event) => {
                  const value = event.target.value;
                  setCamposForm((current) =>
                    current.map((item, itemIndex) =>
                      itemIndex === index ? { ...item, value } : item,
                    ),
                  );
                }}
                disabled={!editable || isSubmitting}
                required
              />
            </label>
          ))}

          {editable ? (
            <div className="ocr-actions">
              <button type="submit" className="btn btn-ghost" disabled={isSubmitting}>
                <RefreshCw size={16} />
                {isSubmitting ? "Guardando…" : "Validar campos"}
              </button>
              {canAplicarOcr(estado) ? (
                <button
                  type="button"
                  className="btn btn-primary"
                  disabled={isSubmitting}
                  onClick={() => void handleAplicar()}
                >
                  <CheckCircle2 size={16} />
                  {isSubmitting ? "Aplicando…" : "Aplicar al gasto"}
                </button>
              ) : null}
            </div>
          ) : null}
        </form>
      ) : null}
    </div>
  );
}
