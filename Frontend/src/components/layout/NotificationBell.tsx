import { useEffect, useRef, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Bell, CheckCheck } from "lucide-react";
import { EmptyState } from "@/components/ui/EmptyState";
import { LoadingState } from "@/components/ui/LoadingState";
import {
  formatNotificationTime,
  getNotificationNavigation,
  type NotificationNavigation,
} from "@/features/notificaciones/notificationUtils";
import { useNotifications } from "@/features/notificaciones/useNotifications";
import { useAuth } from "@/features/auth/AuthContext";

export function NotificationBell() {
  const navigate = useNavigate();
  const { session } = useAuth();
  const panelRef = useRef<HTMLDivElement>(null);
  const [isOpen, setIsOpen] = useState(false);
  const { items, noLeidas, isLoading, markLeida, markAllLeidas } = useNotifications();

  useEffect(() => {
    if (!isOpen) return;

    function handlePointerDown(event: MouseEvent) {
      if (!panelRef.current?.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }

    function handleEscape(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setIsOpen(false);
      }
    }

    document.addEventListener("mousedown", handlePointerDown);
    document.addEventListener("keydown", handleEscape);
    return () => {
      document.removeEventListener("mousedown", handlePointerDown);
      document.removeEventListener("keydown", handleEscape);
    };
  }, [isOpen]);

  async function handleNotificationClick(id: string, target: NotificationNavigation | null) {
    const item = items.find((entry) => entry.id === id);
    if (item && !item.leida) {
      await markLeida(id);
    }

    setIsOpen(false);
    if (target) {
      navigate(target.pathname, { state: target.state });
    }
  }

  async function handleMarkAll() {
    await markAllLeidas();
  }

  const badgeLabel = noLeidas > 99 ? "99+" : String(noLeidas);

  return (
    <div className="notification-bell-wrap" ref={panelRef}>
      <button
        type="button"
        className={`topbar-notification${isOpen ? " is-open" : ""}`}
        aria-label="Notificaciones"
        aria-expanded={isOpen}
        aria-haspopup="true"
        onClick={() => setIsOpen((current) => !current)}
      >
        <Bell size={18} />
        {noLeidas > 0 ? (
          <span className="topbar-notification-badge" aria-hidden="true">
            {badgeLabel}
          </span>
        ) : (
          <span
            className="topbar-notification-badge topbar-notification-badge-empty"
            aria-hidden="true"
          >
            0
          </span>
        )}
      </button>

      {isOpen ? (
        <>
          <button
            type="button"
            className="notification-backdrop"
            aria-label="Cerrar notificaciones"
            onClick={() => setIsOpen(false)}
          />
          <div className="notification-panel" role="dialog" aria-label="Notificaciones">
          <div className="notification-panel-header">
            <div>
              <strong>Notificaciones</strong>
              <span className="table-meta">
                {noLeidas > 0 ? `${noLeidas} sin leer` : "Estás al día"}
              </span>
            </div>
            {noLeidas > 0 ? (
              <button type="button" className="btn btn-ghost notification-mark-all" onClick={() => void handleMarkAll()}>
                <CheckCheck size={16} />
                Marcar todas
              </button>
            ) : null}
          </div>

          {isLoading && items.length === 0 ? (
            <LoadingState label="Cargando notificaciones…" skeletonRows={2} />
          ) : null}

          {!isLoading && items.length === 0 ? (
            <EmptyState
              title="Sin notificaciones"
              description="Aquí verás avisos sobre legalizaciones, gastos y trámites."
            />
          ) : null}

          {items.length > 0 ? (
            <div className="notification-list">
              {items.map((item) => {
                const target = getNotificationNavigation(item, session?.rol);

                return (
                  <button
                    key={item.id}
                    type="button"
                    className={`notification-item${item.leida ? "" : " is-unread"}`}
                    onClick={() => void handleNotificationClick(item.id, target)}
                  >
                    <div className="notification-item-header">
                      <strong>{item.titulo}</strong>
                      <span className="notification-time">
                        {formatNotificationTime(item.createdAt)}
                      </span>
                    </div>
                    <p className="notification-message">{item.mensaje}</p>
                    {target ? (
                      <span className="notification-link-hint">Ver legalización</span>
                    ) : null}
                  </button>
                );
              })}
            </div>
          ) : null}

          <div className="notification-panel-footer">
            <Link className="dashboard-link" to="/" onClick={() => setIsOpen(false)}>
              Ir al inicio
            </Link>
          </div>
          </div>
        </>
      ) : null}
    </div>
  );
}
