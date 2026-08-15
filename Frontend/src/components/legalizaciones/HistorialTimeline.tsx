import { EstadoBadge } from "@/components/legalizaciones/EstadoBadge";
import { formatDateTime } from "@/features/legalizaciones/legalizacionUtils";
import { parseEstadoLegalizacion } from "@/features/legalizaciones/workflowUtils";
import type { LegalizacionHistorial } from "@/types/legalizacion";

type HistorialTimelineProps = {
  items: LegalizacionHistorial[];
};

export function HistorialTimeline({ items }: HistorialTimelineProps) {
  if (items.length === 0) {
    return (
      <div className="empty-state">
        <strong>Sin movimientos registrados</strong>
        <p>El historial aparecerá cuando la legalización cambie de estado.</p>
      </div>
    );
  }

  return (
    <ol className="timeline">
      {items.map((entry) => {
        const estadoAnterior = parseEstadoLegalizacion(entry.estadoAnterior);
        const estadoNuevo = parseEstadoLegalizacion(entry.estadoNuevo);

        return (
          <li key={entry.id} className="timeline-item">
            <div className="timeline-marker" aria-hidden="true" />
            <div className="timeline-content">
              <div className="row-title">
                {estadoAnterior ? (
                  <>
                    <EstadoBadge estado={estadoAnterior} />
                    <span className="timeline-arrow">→</span>
                  </>
                ) : null}
                {estadoNuevo ? <EstadoBadge estado={estadoNuevo} /> : null}
              </div>
              <span className="table-meta">{formatDateTime(entry.createdAt)}</span>
              {entry.comentario ? <p className="timeline-comment">{entry.comentario}</p> : null}
            </div>
          </li>
        );
      })}
    </ol>
  );
}
