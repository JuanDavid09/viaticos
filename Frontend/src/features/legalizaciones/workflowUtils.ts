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
  return action === "rechazar" || action === "cerrar" || action === "reabrir";
}

export function requiresWorkflowConfirmation(action: WorkflowAction): boolean {
  return action !== "aprobar";
}

export function getWorkflowConfirmMessage(action: WorkflowAction): { title: string; message: string } {
  switch (action) {
    case "enviar-validacion":
      return {
        title: "Enviar a validación",
        message: "La legalización pasará a validación. Revisa que los gastos estén completos antes de continuar.",
      };
    case "enviar-aprobacion":
      return {
        title: "Enviar a aprobación",
        message: "Se enviará al jefe aprobador. Ya no podrás modificar los datos del viaje.",
      };
    case "aprobar":
      return {
        title: "Aprobar legalización",
        message: "¿Confirmas que apruebas esta legalización?",
      };
    case "reabrir":
      return {
        title: "Reabrir borrador",
        message: "La legalización volverá a borrador para que el empleado pueda corregirla.",
      };
    case "enviar-nomina":
      return {
        title: "Enviar a nómina",
        message: "Se enviará a nómina para el cierre del expediente.",
      };
    case "cerrar":
      return {
        title: "Cerrar legalización",
        message: "Esta acción cierra la legalización de forma definitiva.",
      };
    default:
      return { title: "Confirmar acción", message: "¿Deseas continuar?" };
  }
}

export function getAvailableWorkflowActions(
  legalizacion: LegalizacionDetalle,
  rol: UserRole,
  userId: string,
): WorkflowAction[] {
  const isOwner = legalizacion.empleadoId === userId;
  const { estado } = legalizacion;
  const actions: WorkflowAction[] = [];

  if (isOwner) {
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

  if ((rol === "JEFE_APROBADOR" || rol === "ADMIN") && estado === "PendienteAprobacion" && !isOwner) {
    actions.push("aprobar", "rechazar");
  }

  if ((rol === "NOMINA" || rol === "ADMIN") && estado === "PendienteNomina") {
    actions.push("cerrar");
  }

  return actions;
}

export function isWorkflowAction(value: string): value is WorkflowAction {
  return value in actionLabels;
}

export function parseEstadoLegalizacion(value: string | null): EstadoLegalizacion | null {
  if (!value) return null;
  return value as EstadoLegalizacion;
}
