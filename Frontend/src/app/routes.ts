import type { UserRole } from "@/types/auth";

export const appRoutes = {
  home: "/",
  login: "/login",
  cambiarClave: "/cambiar-clave",
  usuarios: "/usuarios",
  legalizaciones: "/legalizaciones",
  bandejas: "/bandejas",
  reportes: "/reportes",
  soportes: "/soportes",
} as const;

export type AppRoute = (typeof appRoutes)[keyof typeof appRoutes];

export type NavRoute = Exclude<AppRoute, typeof appRoutes.login | typeof appRoutes.cambiarClave>;

export type NavItem = {
  to: NavRoute;
  label: string;
  group: string;
  roles: readonly UserRole[];
};

export const navItems: NavItem[] = [
  {
    to: appRoutes.home,
    label: "Inicio",
    group: "General",
    roles: ["EMPLEADO", "JEFE_APROBADOR", "NOMINA", "ADMIN"],
  },
  {
    to: appRoutes.legalizaciones,
    label: "Mis legalizaciones",
    group: "Empleado",
    roles: ["EMPLEADO", "JEFE_APROBADOR", "ADMIN"],
  },
  {
    to: appRoutes.bandejas,
    label: "Bandejas",
    group: "Aprobación",
    roles: ["JEFE_APROBADOR", "NOMINA", "ADMIN"],
  },
  {
    to: appRoutes.reportes,
    label: "Reportes",
    group: "Análisis",
    roles: ["JEFE_APROBADOR", "NOMINA", "ADMIN"],
  },
  {
    to: appRoutes.soportes,
    label: "Soportes y OCR",
    group: "Documentos",
    roles: ["EMPLEADO", "ADMIN"],
  },
  {
    to: appRoutes.usuarios,
    label: "Usuarios",
    group: "Administración",
    roles: ["ADMIN"],
  },
];

export function getNavItemsForRole(rol: UserRole): NavItem[] {
  return navItems.filter((item) => item.roles.includes(rol) || rol === "ADMIN");
}

export function getRouteRoles(path: string): readonly UserRole[] | null {
  const item = navItems.find((entry) => entry.to === path);
  return item?.roles ?? null;
}
