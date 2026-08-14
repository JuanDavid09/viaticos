import type { FormEvent } from "react";
import type { Catalogos } from "@/types/catalogos";
import type { LegalizacionFormValues } from "@/types/legalizacion";

type LegalizacionFormProps = {
  form: LegalizacionFormValues;
  catalogos: Catalogos | null;
  isSubmitting: boolean;
  submitLabel: string;
  onChange: (form: LegalizacionFormValues) => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
};

export function LegalizacionForm({
  form,
  catalogos,
  isSubmitting,
  submitLabel,
  onChange,
  onSubmit,
}: LegalizacionFormProps) {
  return (
    <form className="stack-form" onSubmit={onSubmit}>
      <label>
        Motivo del viaje
        <textarea
          value={form.motivo}
          onChange={(event) => onChange({ ...form, motivo: event.target.value })}
          rows={3}
          required
          disabled={isSubmitting}
        />
      </label>

      <label>
        Destino
        <input
          value={form.destino}
          onChange={(event) => onChange({ ...form, destino: event.target.value })}
          placeholder="Ciudad o lugar"
          disabled={isSubmitting}
        />
      </label>

      <div className="form-row">
        <label>
          Fecha inicio
          <input
            type="date"
            value={form.fechaInicio}
            onChange={(event) => onChange({ ...form, fechaInicio: event.target.value })}
            required
            disabled={isSubmitting}
          />
        </label>
        <label>
          Fecha fin
          <input
            type="date"
            value={form.fechaFin}
            onChange={(event) => onChange({ ...form, fechaFin: event.target.value })}
            required
            disabled={isSubmitting}
          />
        </label>
      </div>

      <div className="form-row">
        <label>
          Moneda
          <select
            value={form.monedaId}
            onChange={(event) => onChange({ ...form, monedaId: event.target.value })}
            required
            disabled={isSubmitting || !catalogos}
          >
            <option value="">Seleccionar…</option>
            {catalogos?.monedas.map((moneda) => (
              <option key={moneda.id} value={moneda.id}>
                {moneda.codigoIso} — {moneda.nombre}
              </option>
            ))}
          </select>
        </label>
        <label>
          Anticipo
          <input
            type="number"
            min="0"
            step="0.01"
            value={form.montoAnticipo}
            onChange={(event) => onChange({ ...form, montoAnticipo: event.target.value })}
            required
            disabled={isSubmitting}
          />
        </label>
      </div>

      <button className="btn btn-primary" type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Guardando…" : submitLabel}
      </button>
    </form>
  );
}
