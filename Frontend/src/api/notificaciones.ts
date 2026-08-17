import { apiRequest } from "@/api/http";
import type { Notificacion, NotificacionResumen } from "@/types/notificacion";

export async function listNotificaciones(limite = 20): Promise<Notificacion[]> {
  return apiRequest<Notificacion[]>(`/api/notificaciones?limite=${limite}`);
}

export async function getNotificacionesResumen(): Promise<NotificacionResumen> {
  return apiRequest<NotificacionResumen>("/api/notificaciones/resumen");
}

export async function marcarNotificacionLeida(id: string): Promise<Notificacion> {
  return apiRequest<Notificacion>(`/api/notificaciones/${id}/leida`, {
    method: "PATCH",
  });
}

export async function marcarTodasNotificacionesLeidas(): Promise<void> {
  await apiRequest<void>("/api/notificaciones/marcar-todas-leidas", {
    method: "POST",
  });
}
