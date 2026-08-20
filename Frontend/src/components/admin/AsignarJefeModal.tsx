import { useEffect, useState, type FormEvent } from "react";
import { X } from "lucide-react";
import type { Empleado } from "@/types/empleado";

type Props = {
  isOpen: boolean;
  empleado: Empleado | null;
  jefesDisponibles: Empleado[];
  isSubmitting: boolean;
  onClose: () => void;
  onSubmit: (jefeId: string) => void;
};

export function AsignarJefeModal({
  isOpen,
  empleado,
  jefesDisponibles,
  isSubmitting,
  onClose,
  onSubmit,
}: Props) {
  const [jefeId, setJefeId] = useState("");

  useEffect(() => {
    if (isOpen && empleado) {
      setJefeId(empleado.jefeId ?? "");
    }
  }, [isOpen, empleado]);

  if (!isOpen || !empleado) return null;

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!jefeId) return;
    onSubmit(jefeId);
  }

  return (
    <div className="modal-backdrop" role="presentation" onClick={onClose}>
      <div
        className="modal-card"
        role="dialog"
        aria-modal="true"
        aria-labelledby="asignar-jefe-title"
        onClick={(event) => event.stopPropagation()}
      >
        <div className="modal-header">
          <h3 id="asignar-jefe-title">Asignar jefe</h3>
          <button type="button" className="btn btn-ghost modal-close" onClick={onClose} aria-label="Cerrar">
            <X size={18} />
          </button>
        </div>

        <p className="table-meta">
          {empleado.nombreCompleto} debe tener un jefe aprobador para que sus legalizaciones aparezcan en
          la bandeja del supervisor.
        </p>

        <form className="stack-form" onSubmit={handleSubmit}>
          <label htmlFor="editar-jefe">
            Jefe aprobador
            <select
              id="editar-jefe"
              value={jefeId}
              onChange={(e) => setJefeId(e.target.value)}
              required
            >
              <option value="">Seleccione un jefe…</option>
              {jefesDisponibles
                .filter((jefe) => jefe.id !== empleado.id)
                .map((jefe) => (
                  <option key={jefe.id} value={jefe.id}>
                    {jefe.nombreCompleto} ({jefe.codigoEmpleado})
                  </option>
                ))}
            </select>
          </label>

          <div className="modal-actions">
            <button type="button" className="btn btn-ghost" onClick={onClose} disabled={isSubmitting}>
              Cancelar
            </button>
            <button type="submit" className="btn btn-primary" disabled={isSubmitting || !jefeId}>
              {isSubmitting ? "Guardando…" : "Guardar"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
