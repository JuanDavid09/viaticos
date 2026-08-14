import { Navigate, Outlet } from "react-router-dom";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";

export function GuestRoute() {
  const { isAuthenticated, isBootstrapping } = useAuth();

  if (isBootstrapping) {
    return (
      <div className="route-loading">
        <p>Cargando…</p>
      </div>
    );
  }

  if (isAuthenticated) {
    return <Navigate to={appRoutes.home} replace />;
  }

  return <Outlet />;
}
