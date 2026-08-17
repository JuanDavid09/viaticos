import { appRoutes } from "@/app/routes";
import type { Notificacion } from "@/types/notificacion";

export function getNotificationLink(notificacion: Notificacion): string | null {
  if (notificacion.entidadTipo === "LEGALIZACION" && notificacion.entidadId) {
    return `${appRoutes.legalizaciones}/${notificacion.entidadId}`;
  }

  return null;
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
