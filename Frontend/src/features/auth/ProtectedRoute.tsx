import { Navigate, Outlet, useLocation } from "react-router-dom";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";

export function ProtectedRoute() {
  const { isAuthenticated, isBootstrapping } = useAuth();
  const location = useLocation();

  if (isBootstrapping) {
    return (
      <div className="route-loading">
        <p>Cargando sesión…</p>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to={appRoutes.login} replace state={{ from: location.pathname }} />;
  }

  return <Outlet />;
}
