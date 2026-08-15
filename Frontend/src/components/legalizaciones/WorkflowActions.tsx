import { useState } from "react";
import { CheckCircle2, RotateCcw, Send, XCircle } from "lucide-react";
import { RechazarModal } from "@/components/legalizaciones/RechazarModal";
import {
  getAvailableWorkflowActions,
  getWorkflowActionLabel,
  isWorkflowDangerAction,
} from "@/features/legalizaciones/workflowUtils";
import type { UserRole } from "@/types/auth";
import type { LegalizacionDetalle, WorkflowAction } from "@/types/legalizacion";

type WorkflowActionsProps = {
  legalizacion: LegalizacionDetalle;
  rol: UserRole;
  userId: string;
  isSubmitting: boolean;
  onAction: (action: WorkflowAction, comentario?: string) => Promise<void>;
};

function getActionIcon(action: WorkflowAction) {
  switch (action) {
    case "aprobar":
      return CheckCircle2;
    case "rechazar":
      return XCircle;
    case "reabrir":
      return RotateCcw;
    default:
      return Send;
  }
}

export function WorkflowActions({
  legalizacion,
  rol,
  userId,
  isSubmitting,
  onAction,
}: WorkflowActionsProps) {
  const [showRejectModal, setShowRejectModal] = useState(false);
  const actions = getAvailableWorkflowActions(legalizacion, rol, userId);

  if (actions.length === 0) {
    return null;
  }

  async function handleClick(action: WorkflowAction) {
    if (action === "rechazar") {
      setShowRejectModal(true);
      return;
    }

    await onAction(action);
  }

  async function handleReject(comentario: string) {
    await onAction("rechazar", comentario);
    setShowRejectModal(false);
  }

  return (
    <>
      <section className="workflow-actions card">
        <div>
          <h3>Acciones de flujo</h3>
          <p className="table-meta">Avanza la legalización según tu rol y el estado actual.</p>
        </div>
        <div className="workflow-actions-buttons">
          {actions.map((action) => {
            const Icon = getActionIcon(action);
            const isDanger = isWorkflowDangerAction(action);
            return (
              <button
                key={action}
                type="button"
                className={`btn ${isDanger ? "btn-danger" : "btn-primary"}`}
                disabled={isSubmitting}
                onClick={() => void handleClick(action)}
              >
                <Icon size={16} />
                {isSubmitting ? "Procesando…" : getWorkflowActionLabel(action)}
              </button>
            );
          })}
        </div>
      </section>

      <RechazarModal
        isOpen={showRejectModal}
        isSubmitting={isSubmitting}
        onClose={() => setShowRejectModal(false)}
        onSubmit={(comentario) => void handleReject(comentario)}
      />
    </>
  );
}
