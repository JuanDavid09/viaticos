import type { FormEvent } from "react";
import type { Catalogos } from "@/types/catalogos";
import type { GastoFormValues } from "@/types/legalizacion";

type GastoFormProps = {
  form: GastoFormValues;
  catalogos: Catalogos | null;
  isSubmitting: boolean;
  onChange: (form: GastoFormValues) => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
};

export function GastoForm({
  form,
  catalogos,
  isSubmitting,
  onChange,
  onSubmit,
}: GastoFormProps) {
  return (
    <form className="stack-form" onSubmit={onSubmit}>
      <label htmlFor="gasto-categoria">
        Categoría
        <select
          id="gasto-categoria"
          value={form.categoriaGastoId}
          onChange={(event) => onChange({ ...form, categoriaGastoId: event.target.value })}
          required
          disabled={isSubmitting || !catalogos}
        >
          <option value="">Seleccionar…</option>
          {catalogos?.categorias.map((categoria) => (
            <option key={categoria.id} value={categoria.id}>
              {categoria.nombre}
            </option>
          ))}
        </select>
      </label>

      <div className="form-row">
        <label htmlFor="gasto-fecha">
          Fecha del gasto
          <input
            id="gasto-fecha"
            type="date"
            value={form.fechaGasto}
            onChange={(event) => onChange({ ...form, fechaGasto: event.target.value })}
            required
            disabled={isSubmitting}
          />
        </label>
        <label htmlFor="gasto-monto">
          Monto
          <input
            id="gasto-monto"
            type="number"
            min="0.01"
            step="0.01"
            value={form.monto}
            onChange={(event) => onChange({ ...form, monto: event.target.value })}
            required
            disabled={isSubmitting}
          />
        </label>
      </div>

      <label htmlFor="gasto-descripcion">
        Descripción
        <input
          id="gasto-descripcion"
          value={form.descripcion}
          onChange={(event) => onChange({ ...form, descripcion: event.target.value })}
          required
          disabled={isSubmitting}
        />
      </label>

      <div className="form-row">
        <label htmlFor="gasto-proveedor">
          Proveedor
          <input
            id="gasto-proveedor"
            value={form.proveedor}
            onChange={(event) => onChange({ ...form, proveedor: event.target.value })}
            disabled={isSubmitting}
          />
        </label>
        <label htmlFor="gasto-documento">
          N.º documento
          <input
            id="gasto-documento"
            value={form.numeroDocumento}
            onChange={(event) => onChange({ ...form, numeroDocumento: event.target.value })}
            disabled={isSubmitting}
          />
        </label>
      </div>

      <button className="btn btn-primary" type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Agregando…" : "Agregar gasto"}
      </button>
    </form>
  );
}
