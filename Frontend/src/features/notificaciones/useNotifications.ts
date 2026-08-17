import { useCallback, useEffect, useState } from "react";
import {
  getNotificacionesResumen,
  listNotificaciones,
  marcarNotificacionLeida,
  marcarTodasNotificacionesLeidas,
} from "@/api/notificaciones";
import { useAuth } from "@/features/auth/AuthContext";
import type { Notificacion } from "@/types/notificacion";

const POLL_INTERVAL_MS = 45000;

export function useNotifications() {
  const { isAuthenticated } = useAuth();
  const [items, setItems] = useState<Notificacion[]>([]);
  const [noLeidas, setNoLeidas] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const refresh = useCallback(async () => {
    if (!isAuthenticated) {
      setItems([]);
      setNoLeidas(0);
      return;
    }

    setIsLoading(true);
    try {
      const [notificaciones, resumen] = await Promise.all([
        listNotificaciones(15),
        getNotificacionesResumen(),
      ]);
      setItems(notificaciones);
      setNoLeidas(resumen.noLeidas);
    } catch {
      // Mantener el último estado conocido si falla el polling.
    } finally {
      setIsLoading(false);
    }
  }, [isAuthenticated]);

  useEffect(() => {
    void refresh();

    const intervalId = window.setInterval(() => {
      void refresh();
    }, POLL_INTERVAL_MS);

    function handleFocus() {
      void refresh();
    }

    window.addEventListener("focus", handleFocus);
    return () => {
      window.clearInterval(intervalId);
      window.removeEventListener("focus", handleFocus);
    };
  }, [refresh]);

  const markLeida = useCallback(async (id: string) => {
    const currentItem = items.find((entry) => entry.id === id);
    const updated = await marcarNotificacionLeida(id);
    setItems((current) =>
      current.map((item) => (item.id === updated.id ? updated : item)),
    );
    if (currentItem && !currentItem.leida) {
      setNoLeidas((current) => Math.max(0, current - 1));
    }
  }, [items]);

  const markAllLeidas = useCallback(async () => {
    await marcarTodasNotificacionesLeidas();
    setItems((current) =>
      current.map((item) => ({
        ...item,
        leida: true,
        leidaAt: item.leidaAt ?? new Date().toISOString(),
      })),
    );
    setNoLeidas(0);
  }, []);

  return {
    items,
    noLeidas,
    isLoading,
    refresh,
    markLeida,
    markAllLeidas,
  };
}
