export const appRoutes = {
  home: "/",
  login: "/login",
  legalizaciones: "/legalizaciones",
  bandejas: "/bandejas",
  soportes: "/soportes",
} as const;

export const navItems = [
  { to: appRoutes.home, label: "Inicio", group: "General" },
  { to: appRoutes.legalizaciones, label: "Mis legalizaciones", group: "Empleado" },
  { to: appRoutes.bandejas, label: "Bandejas", group: "Aprobación" },
  { to: appRoutes.soportes, label: "Soportes y OCR", group: "Documentos" },
] as const;
