import type { UserRole } from "@/types/auth";

const roleLabels: Record<UserRole, string> = {
  EMPLEADO: "Empleado",
  JEFE_APROBADOR: "Jefe aprobador",
  NOMINA: "Nómina",
  ADMIN: "Administrador",
};

export function getRoleLabel(rol: UserRole): string {
  return roleLabels[rol];
}

export function hasAnyRole(rol: UserRole, allowed: readonly UserRole[]): boolean {
  if (allowed.length === 0) return true;
  if (rol === "ADMIN") return true;
  return allowed.includes(rol);
}

export function getInitials(name: string): string {
  const parts = name.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return "?";
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
  return `${parts[0][0]}${parts[1][0]}`.toUpperCase();
}

export const demoUsers = [
  { email: "empleado@empresa.com", label: "Empleado" },
  { email: "jefe@empresa.com", label: "Jefe" },
  { email: "nomina@empresa.com", label: "Nómina" },
  { email: "admin@empresa.com", label: "Admin" },
] as const;
