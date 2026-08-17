import type { ReporteDefinition, ReporteFiltros } from "@/types/reportes";
import type { EstadoLegalizacion } from "@/types/legalizacion";

const estadoOptions: { value: EstadoLegalizacion; label: string }[] = [
  { value: "Borrador", label: "Borrador" },
  { value: "PendienteValidacion", label: "Pendiente validación" },
  { value: "PendienteAprobacion", label: "Pendiente aprobación" },
  { value: "Aprobada", label: "Aprobada" },
  { value: "Rechazada", label: "Rechazada" },
  { value: "PendienteNomina", label: "Pendiente nómina" },
  { value: "Cerrada", label: "Cerrada" },
];

type ReporteFiltersProps = {
  definition: ReporteDefinition;
  filtros: ReporteFiltros;
  isLoading: boolean;
  onChange: (filtros: ReporteFiltros) => void;
  onSubmit: () => void;
};

export function ReporteFilters({
  definition,
  filtros,
  isLoading,
  onChange,
  onSubmit,
}: ReporteFiltersProps) {
  const currentYear = new Date().getFullYear();

  return (
    <form
      className="report-filters"
      onSubmit={(event) => {
        event.preventDefault();
        onSubmit();
      }}
    >
      {definition.supportsDateFilter ? (
        <div className="form-row">
          <label htmlFor="reporte-desde">
            Desde
            <input
              id="reporte-desde"
              type="date"
              value={filtros.desde ?? ""}
              onChange={(event) => onChange({ ...filtros, desde: event.target.value || undefined })}
              disabled={isLoading}
            />
          </label>
          <label htmlFor="reporte-hasta">
            Hasta
            <input
              id="reporte-hasta"
              type="date"
              value={filtros.hasta ?? ""}
              onChange={(event) => onChange({ ...filtros, hasta: event.target.value || undefined })}
              disabled={isLoading}
            />
          </label>
        </div>
      ) : null}

      {definition.supportsDepartamento ? (
        <label htmlFor="reporte-departamento">
          Departamento
          <input
            id="reporte-departamento"
            value={filtros.departamento ?? ""}
            onChange={(event) =>
              onChange({ ...filtros, departamento: event.target.value || undefined })
            }
            placeholder="Ej. Operaciones"
            disabled={isLoading}
          />
        </label>
      ) : null}

      {definition.supportsEstado ? (
        <label htmlFor="reporte-estado">
          Estado
          <select
            id="reporte-estado"
            value={filtros.estado ?? ""}
            onChange={(event) =>
              onChange({
                ...filtros,
                estado: (event.target.value as EstadoLegalizacion) || undefined,
              })
            }
            disabled={isLoading}
          >
            <option value="">Todos</option>
            {estadoOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </label>
      ) : null}

      {definition.supportsAnio ? (
        <label htmlFor="reporte-anio">
          Año
          <input
            id="reporte-anio"
            type="number"
            min="2020"
            max={currentYear + 1}
            value={filtros.anio ?? currentYear}
            onChange={(event) =>
              onChange({ ...filtros, anio: Number(event.target.value) || currentYear })
            }
            disabled={isLoading}
          />
        </label>
      ) : null}

      {definition.supportsSoloCerradas ? (
        <label className="checkbox-inline" htmlFor="reporte-solo-cerradas">
          <input
            id="reporte-solo-cerradas"
            type="checkbox"
            checked={filtros.soloCerradas ?? true}
            onChange={(event) => onChange({ ...filtros, soloCerradas: event.target.checked })}
            disabled={isLoading}
          />
          Solo legalizaciones cerradas
        </label>
      ) : null}

      <button className="btn btn-primary" type="submit" disabled={isLoading}>
        {isLoading ? "Generando…" : "Generar reporte"}
      </button>
    </form>
  );
}
