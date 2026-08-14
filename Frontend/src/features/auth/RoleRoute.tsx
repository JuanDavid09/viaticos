import { Navigate, Outlet } from "react-router-dom";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";
import type { UserRole } from "@/types/auth";

type RoleRouteProps = {
  allowedRoles: readonly UserRole[];
};

export function RoleRoute({ allowedRoles }: RoleRouteProps) {
  const { hasRole } = useAuth();

  if (!hasRole(...allowedRoles)) {
    return <Navigate to={appRoutes.home} replace />;
  }

  return <Outlet />;
}
