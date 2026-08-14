import { Navigate, Route, Routes } from "react-router-dom";
import { AppShell } from "@/components/layout/AppShell";
import { appRoutes, getRouteRoles } from "@/app/routes";
import { GuestRoute } from "@/features/auth/GuestRoute";
import { ProtectedRoute } from "@/features/auth/ProtectedRoute";
import { RequirePasswordChanged } from "@/features/auth/RequirePasswordChanged";
import { RoleRoute } from "@/features/auth/RoleRoute";
import { HomePage } from "@/pages/HomePage";
import { LoginPage } from "@/pages/LoginPage";
import { ChangePasswordPage } from "@/pages/ChangePasswordPage";
import { LegalizacionesPage } from "@/pages/LegalizacionesPage";
import { LegalizacionNuevaPage } from "@/pages/LegalizacionNuevaPage";
import { LegalizacionDetallePage } from "@/pages/LegalizacionDetallePage";
import { BandejasPage } from "@/pages/BandejasPage";
import { SoportesPage } from "@/pages/SoportesPage";
import { UsuariosPage } from "@/pages/UsuariosPage";

export function AppRouter() {
  return (
    <Routes>
      <Route element={<GuestRoute />}>
        <Route path={appRoutes.login} element={<LoginPage />} />
      </Route>

      <Route element={<ProtectedRoute />}>
        <Route path={appRoutes.cambiarClave} element={<ChangePasswordPage />} />

        <Route element={<RequirePasswordChanged />}>
          <Route element={<AppShell />}>
            <Route path={appRoutes.home} element={<HomePage />} />

            <Route
              element={<RoleRoute allowedRoles={getRouteRoles(appRoutes.legalizaciones) ?? []} />}
            >
              <Route path={appRoutes.legalizaciones} element={<LegalizacionesPage />} />
              <Route path={`${appRoutes.legalizaciones}/nueva`} element={<LegalizacionNuevaPage />} />
              <Route path={`${appRoutes.legalizaciones}/:id`} element={<LegalizacionDetallePage />} />
            </Route>

            <Route element={<RoleRoute allowedRoles={getRouteRoles(appRoutes.bandejas) ?? []} />}>
              <Route path={appRoutes.bandejas} element={<BandejasPage />} />
            </Route>

            <Route element={<RoleRoute allowedRoles={getRouteRoles(appRoutes.soportes) ?? []} />}>
              <Route path={appRoutes.soportes} element={<SoportesPage />} />
            </Route>

            <Route element={<RoleRoute allowedRoles={getRouteRoles(appRoutes.usuarios) ?? []} />}>
              <Route path={appRoutes.usuarios} element={<UsuariosPage />} />
            </Route>
          </Route>
        </Route>
      </Route>

      <Route path="*" element={<Navigate to={appRoutes.home} replace />} />
    </Routes>
  );
}
