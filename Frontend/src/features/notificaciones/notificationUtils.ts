import { appRoutes } from "@/app/routes";
import type { UserRole } from "@/types/auth";
import type { Notificacion } from "@/types/notificacion";

const SUPERVISOR_NOTIFICATION_TYPES = new Set([
  "LEGALIZACION_CREADA",
  "GASTO_AGREGADO",
  "ENVIADA_VALIDACION",
  "ENVIADA_APROBACION",
  "ENVIADA_NOMINA",
]);

export type NotificationNavigation = {
  pathname: string;
  state?: { fromBandejas?: boolean };
};

export function getNotificationNavigation(
  notificacion: Notificacion,
  rol?: UserRole,
): NotificationNavigation | null {
  if (notificacion.entidadTipo !== "LEGALIZACION" || !notificacion.entidadId) {
    return null;
  }

  const pathname = `${appRoutes.legalizaciones}/${notificacion.entidadId}`;
  const isSupervisor =
    rol === "ADMIN" || rol === "JEFE_APROBADOR" || rol === "NOMINA";
  const fromBandejas =
    isSupervisor && SUPERVISOR_NOTIFICATION_TYPES.has(notificacion.tipo);

  return fromBandejas ? { pathname, state: { fromBandejas: true } } : { pathname };
}

export function formatNotificationTime(value: string): string {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;

  const diffMs = Date.now() - date.getTime();
  const diffMinutes = Math.floor(diffMs / 60000);

  if (diffMinutes < 1) return "Ahora";
  if (diffMinutes < 60) return `Hace ${diffMinutes} min`;

  const diffHours = Math.floor(diffMinutes / 60);
  if (diffHours < 24) return `Hace ${diffHours} h`;

  return new Intl.DateTimeFormat("es-CO", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(date);
}
