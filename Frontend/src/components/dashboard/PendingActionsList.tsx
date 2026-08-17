import { Link } from "react-router-dom";
import { ChevronRight } from "lucide-react";
import { EstadoBadge } from "@/components/legalizaciones/EstadoBadge";
import { EmptyState } from "@/components/ui/EmptyState";
import { appRoutes } from "@/app/routes";
import type { EmployeePendingItem } from "@/features/dashboard/employeeDashboard";

type PendingActionsListProps = {
  items: EmployeePendingItem[];
};

export function PendingActionsList({ items }: PendingActionsListProps) {
  if (items.length === 0) {
    return (
      <EmptyState
        title="Sin pendientes por ahora"
        description="Cuando tengas borradores o legalizaciones que requieran acción, aparecerán aquí."
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
            <span className="dashboard-action-hint">{item.actionHint}</span>
          </div>
          <div className="dashboard-list-action">
            <span>{item.actionLabel}</span>
            <ChevronRight size={16} />
          </div>
        </Link>
      ))}
    </div>
  );
}
