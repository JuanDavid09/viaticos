import type { UserRole } from "@/types/auth";
import type {
  EstadoLegalizacion,
  LegalizacionDetalle,
  WorkflowAction,
} from "@/types/legalizacion";

const actionLabels: Record<WorkflowAction, string> = {
  "enviar-validacion": "Enviar a validación",
  "enviar-aprobacion": "Enviar a aprobación",
  aprobar: "Aprobar",
  rechazar: "Rechazar",
  reabrir: "Reabrir borrador",
  "enviar-nomina": "Enviar a nómina",
  cerrar: "Cerrar legalización",
};

export function getWorkflowActionLabel(action: WorkflowAction): string {
  return actionLabels[action];
}

export function isWorkflowDangerAction(action: WorkflowAction): boolean {
  return action === "rechazar";
}

export function getAvailableWorkflowActions(
  legalizacion: LegalizacionDetalle,
  rol: UserRole,
  userId: string,
): WorkflowAction[] {
  const isOwner = legalizacion.empleadoId === userId;
  const { estado } = legalizacion;
  const actions: WorkflowAction[] = [];

  if (isOwner || rol === "ADMIN") {
    if (estado === "Borrador" && legalizacion.gastos.length > 0) {
      actions.push("enviar-validacion");
    }
    if (estado === "PendienteValidacion") {
      actions.push("enviar-aprobacion");
    }
    if (estado === "Rechazada") {
      actions.push("reabrir");
    }
    if (estado === "Aprobada") {
      actions.push("enviar-nomina");
    }
  }

  if ((rol === "JEFE_APROBADOR" || rol === "ADMIN") && estado === "PendienteAprobacion") {
    actions.push("aprobar", "rechazar");
  }

  if ((rol === "NOMINA" || rol === "ADMIN") && estado === "PendienteNomina") {
    actions.push("cerrar");
  }

  return actions;
}

export function parseEstadoLegalizacion(value: string | null): EstadoLegalizacion | null {
  if (!value) return null;
  return value as EstadoLegalizacion;
}
