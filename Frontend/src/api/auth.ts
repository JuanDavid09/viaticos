import { apiRequest } from "@/api/http";
import type { AuthSession, ChangePasswordRequest, LoginRequest } from "@/types/auth";
import { mapLoginResponse } from "@/types/auth";

export async function login(request: LoginRequest): Promise<AuthSession> {
  const response = await apiRequest<Omit<AuthSession, "mustChangePassword"> & {
    mustChangePassword?: boolean;
  }>("/api/auth/login", {
    method: "POST",
    body: request,
    auth: false,
  });
  return mapLoginResponse(response);
}

export async function changePassword(
  request: ChangePasswordRequest,
  accessToken: string,
): Promise<AuthSession> {
  const response = await apiRequest<Omit<AuthSession, "mustChangePassword"> & {
    mustChangePassword?: boolean;
  }>("/api/auth/change-password", {
    method: "POST",
    body: request,
    accessToken,
  });
  return mapLoginResponse(response);
}
