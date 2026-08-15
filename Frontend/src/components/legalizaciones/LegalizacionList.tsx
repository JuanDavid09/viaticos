import { Link } from "react-router-dom";
import { ChevronRight } from "lucide-react";
import { EstadoBadge } from "@/components/legalizaciones/EstadoBadge";
import { appRoutes } from "@/app/routes";
import { formatDate, formatMoney } from "@/features/legalizaciones/legalizacionUtils";
import type { LegalizacionResumen } from "@/types/legalizacion";

type LegalizacionListProps = {
  items: LegalizacionResumen[];
  fromBandejas?: boolean;
  emptyTitle?: string;
  emptyDescription?: string;
};

export function LegalizacionList({
  items,
  fromBandejas = false,
  emptyTitle = "Sin legalizaciones todavía",
  emptyDescription = "Crea tu primera legalización para registrar un viaje y sus gastos.",
}: LegalizacionListProps) {
  if (items.length === 0) {
    return (
      <div className="empty-state">
        <strong>{emptyTitle}</strong>
        <p>{emptyDescription}</p>
      </div>
    );
  }

  return (
    <div className="table-list">
      {items.map((item) => (
        <Link
          key={item.id}
          to={`${appRoutes.legalizaciones}/${item.id}`}
          state={{ fromBandejas }}
          className="table-row table-row-link"
        >
          <div>
            <div className="row-title">
              <strong>{item.numero}</strong>
              <EstadoBadge estado={item.estado} />
            </div>
            <span className="table-meta">{item.motivo}</span>
            <span className="table-meta">
              {formatDate(item.fechaInicio)} → {formatDate(item.fechaFin)}
              {item.destino ? ` · ${item.destino}` : ""}
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
