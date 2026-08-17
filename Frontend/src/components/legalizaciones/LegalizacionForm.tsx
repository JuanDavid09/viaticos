import type { FormEvent } from "react";
import type { Catalogos } from "@/types/catalogos";
import type { Empleado } from "@/types/empleado";
import type { LegalizacionFormValues } from "@/types/legalizacion";

type LegalizacionFormProps = {
  form: LegalizacionFormValues;
  catalogos: Catalogos | null;
  allowEmpleadoAsignacion?: boolean;
  empleadosAsignables?: Empleado[] | null;
  isSubmitting: boolean;
  submitLabel: string;
  onChange: (form: LegalizacionFormValues) => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
};

export function LegalizacionForm({
  form,
  catalogos,
  allowEmpleadoAsignacion = false,
  empleadosAsignables,
  isSubmitting,
  submitLabel,
  onChange,
  onSubmit,
}: LegalizacionFormProps) {
  return (
    <form className="stack-form" onSubmit={onSubmit}>
      {allowEmpleadoAsignacion ? (
        <label htmlFor="legalizacion-empleado">
          Titular de la legalización
          <select
            id="legalizacion-empleado"
            value={form.empleadoId}
            onChange={(event) => onChange({ ...form, empleadoId: event.target.value })}
            disabled={isSubmitting}
          >
            <option value="">Para mí (mi legalización)</option>
            {empleadosAsignables?.map((empleado) => (
              <option key={empleado.id} value={empleado.id}>
                {empleado.nombreCompleto}
                {empleado.departamento ? ` — ${empleado.departamento}` : ""}
              </option>
            ))}
          </select>
        </label>
      ) : null}

      <label htmlFor="legalizacion-motivo">
        Motivo del viaje
        <textarea
          id="legalizacion-motivo"
          value={form.motivo}
          onChange={(event) => onChange({ ...form, motivo: event.target.value })}
          rows={3}
          required
          disabled={isSubmitting}
        />
      </label>

      <label htmlFor="legalizacion-destino">
        Destino
        <input
          id="legalizacion-destino"
          value={form.destino}
          onChange={(event) => onChange({ ...form, destino: event.target.value })}
          placeholder="Ciudad o lugar"
          disabled={isSubmitting}
        />
      </label>

      <div className="form-row">
        <label htmlFor="legalizacion-fecha-inicio">
          Fecha inicio
          <input
            id="legalizacion-fecha-inicio"
            type="date"
            value={form.fechaInicio}
            onChange={(event) => onChange({ ...form, fechaInicio: event.target.value })}
            required
            disabled={isSubmitting}
          />
        </label>
        <label htmlFor="legalizacion-fecha-fin">
          Fecha fin
          <input
            id="legalizacion-fecha-fin"
            type="date"
            value={form.fechaFin}
            onChange={(event) => onChange({ ...form, fechaFin: event.target.value })}
            required
            disabled={isSubmitting}
          />
        </label>
      </div>

      <div className="form-row">
        <label htmlFor="legalizacion-moneda">
          Moneda
          <select
            id="legalizacion-moneda"
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
        <label htmlFor="legalizacion-anticipo">
          Anticipo
          <input
            id="legalizacion-anticipo"
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
