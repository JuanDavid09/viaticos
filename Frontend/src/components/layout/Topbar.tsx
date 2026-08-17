import { LogOut } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { NotificationBell } from "@/components/layout/NotificationBell";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";
import { getInitials, getRoleLabel } from "@/features/auth/roleUtils";

type TopbarProps = {
  title: string;
  kicker?: string;
};

export function Topbar({ title, kicker }: TopbarProps) {
  const navigate = useNavigate();
  const { session, logout } = useAuth();

  function handleLogout() {
    logout();
    navigate(appRoutes.login, { replace: true });
  }

  return (
    <header className="topbar">
      <div className="topbar-heading">
        {kicker ? <p className="page-kicker">{kicker}</p> : null}
        <h2 className="page-title topbar-title">{title}</h2>
      </div>

      {session ? (
        <div className="topbar-actions">
          <div className="topbar-toolbar">
            <div className="topbar-toolbar-cluster">
              <NotificationBell />

              <span className="topbar-divider topbar-divider-inline" aria-hidden="true" />

              <div className="topbar-user-card" title={session.email}>
                <span className="avatar avatar-lg topbar-avatar" aria-hidden="true">
                  {getInitials(session.nombreCompleto)}
                </span>
                <div className="topbar-user-meta">
                  <strong>{session.nombreCompleto}</strong>
                  <span className="topbar-user-email">{session.email}</span>
                  <span className="topbar-user-role">{getRoleLabel(session.rol)}</span>
                </div>
                <span className="topbar-status-dot" title="Sesión activa" aria-hidden="true" />
              </div>
            </div>

            <span className="topbar-divider topbar-divider-block" aria-hidden="true" />

            <button
              type="button"
              className="btn topbar-logout-btn"
              onClick={handleLogout}
              aria-label="Cerrar sesión"
              title="Cerrar sesión"
            >
              <LogOut size={18} />
              <span className="topbar-logout-label">Salir</span>
            </button>
          </div>
        </div>
      ) : null}
    </header>
  );
}
