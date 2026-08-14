import { Navigate, Outlet } from "react-router-dom";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";

export function RequirePasswordChanged() {
  const { mustChangePassword } = useAuth();

  if (mustChangePassword) {
    return <Navigate to={appRoutes.cambiarClave} replace />;
  }

  return <Outlet />;
}
