import { Link } from "react-router-dom";
import { ChevronRight } from "lucide-react";
import { EstadoBadge } from "@/components/legalizaciones/EstadoBadge";
import { EmptyState } from "@/components/ui/EmptyState";
import { appRoutes } from "@/app/routes";
import { formatDate, formatMoney } from "@/features/legalizaciones/legalizacionUtils";
import type { LegalizacionResumen } from "@/types/legalizacion";

type RecentLegalizacionesProps = {
  items: LegalizacionResumen[];
};

export function RecentLegalizaciones({ items }: RecentLegalizacionesProps) {
  if (items.length === 0) {
    return (
      <EmptyState
        title="Aún no tienes legalizaciones"
        description="Crea tu primera legalización para registrar un viaje y sus gastos."
        action={
          <Link className="btn btn-primary" to={`${appRoutes.legalizaciones}/nueva`}>
            Nueva legalización
          </Link>
        }
      />
    );
  }

  return (
    <div className="dashboard-list">
      {items.map((item) => (
        <Link
          key={item.id}
          to={`${appRoutes.legalizaciones}/${item.id}`}
          className="dashboard-list-item"
        >
          <div>
            <div className="row-title">
              <strong>{item.numero}</strong>
              <EstadoBadge estado={item.estado} />
            </div>
            <span className="table-meta">{item.motivo}</span>
            <span className="table-meta">
              {formatDate(item.fechaInicio)} → {formatDate(item.fechaFin)}
            </span>
          </div>
          <div className="row-summary">
            <span>{formatMoney(item.totalGastos)}</span>
            <ChevronRight size={18} />
          </div>
        </Link>
      ))}
    </div>
  );
}
