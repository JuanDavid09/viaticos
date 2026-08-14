import { apiRequest } from "@/api/http";
import type {
  ActualizarEmpleadoRequest,
  CrearEmpleadoRequest,
  Empleado,
  RestablecerPasswordRequest,
} from "@/types/empleado";

export async function listEmpleados(includeInactive = false): Promise<Empleado[]> {
  const query = includeInactive ? "?includeInactive=true" : "";
  return apiRequest<Empleado[]>(`/api/empleados${query}`);
}

export async function getEmpleado(id: string): Promise<Empleado> {
  return apiRequest<Empleado>(`/api/empleados/${id}`);
}

export async function createEmpleado(request: CrearEmpleadoRequest): Promise<Empleado> {
  return apiRequest<Empleado>("/api/empleados", {
    method: "POST",
    body: request,
  });
}

export async function updateEmpleado(
  id: string,
  request: ActualizarEmpleadoRequest,
): Promise<Empleado> {
  return apiRequest<Empleado>(`/api/empleados/${id}`, {
    method: "PUT",
    body: request,
  });
}

export async function resetEmpleadoPassword(
  id: string,
  request: RestablecerPasswordRequest,
): Promise<Empleado> {
  return apiRequest<Empleado>(`/api/empleados/${id}/restablecer-password`, {
    method: "POST",
    body: request,
  });
}
