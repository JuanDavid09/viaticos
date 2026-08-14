import type { UserRole } from "@/types/auth";

export type Empleado = {
  id: string;
  codigoEmpleado: string;
  email: string;
  nombre: string;
  apellido: string;
  nombreCompleto: string;
  departamento: string | null;
  rol: UserRole;
  jefeId: string | null;
  activo: boolean;
  mustChangePassword: boolean;
};

export type CrearEmpleadoRequest = {
  codigoEmpleado: string;
  email: string;
  nombre: string;
  apellido: string;
  rol: UserRole;
  passwordTemporal: string;
  departamento?: string;
  jefeId?: string;
};

export type ActualizarEmpleadoRequest = {
  nombre: string;
  apellido: string;
  rol: UserRole;
  departamento?: string;
  jefeId?: string | null;
  activo: boolean;
};

export type RestablecerPasswordRequest = {
  passwordTemporal: string;
};

export const roleOptions: { value: UserRole; label: string }[] = [
  { value: "EMPLEADO", label: "Empleado" },
  { value: "JEFE_APROBADOR", label: "Jefe aprobador" },
  { value: "NOMINA", label: "Nómina" },
  { value: "ADMIN", label: "Administrador" },
];
