import { LogOut } from "lucide-react";
import { useNavigate } from "react-router-dom";
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
      <div>
        {kicker ? <p className="page-kicker">{kicker}</p> : null}
        <h2 className="page-title" style={{ marginBottom: 0, fontSize: "1.15rem" }}>
          {title}
        </h2>
      </div>

      {session ? (
        <div className="topbar-actions">
          <div className="user-chip" title={session.email}>
            <span className="avatar">{getInitials(session.nombreCompleto)}</span>
            <span className="user-chip-text">
              <strong>{session.nombreCompleto}</strong>
              <span className="role-badge">{getRoleLabel(session.rol)}</span>
            </span>
          </div>
          <button
            type="button"
            className="btn btn-ghost topbar-logout"
            onClick={handleLogout}
            aria-label="Cerrar sesión"
          >
            <LogOut size={16} />
            Salir
          </button>
        </div>
      ) : null}
    </header>
  );
}
