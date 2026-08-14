import { Navigate, Route, Routes } from "react-router-dom";
import { AppShell } from "@/components/layout/AppShell";
import { appRoutes } from "@/app/routes";
import { HomePage } from "@/pages/HomePage";
import { LoginPage } from "@/pages/LoginPage";
import { LegalizacionesPage } from "@/pages/LegalizacionesPage";
import { BandejasPage } from "@/pages/BandejasPage";
import { SoportesPage } from "@/pages/SoportesPage";

export function AppRouter() {
  return (
    <Routes>
      <Route path={appRoutes.login} element={<LoginPage />} />
      <Route element={<AppShell />}>
        <Route path={appRoutes.home} element={<HomePage />} />
        <Route path={appRoutes.legalizaciones} element={<LegalizacionesPage />} />
        <Route path={appRoutes.bandejas} element={<BandejasPage />} />
        <Route path={appRoutes.soportes} element={<SoportesPage />} />
      </Route>
      <Route path="*" element={<Navigate to={appRoutes.home} replace />} />
    </Routes>
  );
}
