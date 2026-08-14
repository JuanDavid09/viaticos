import { apiRequest } from "@/api/http";
import type { AuthSession, LoginRequest } from "@/types/auth";

export async function login(request: LoginRequest): Promise<AuthSession> {
  return apiRequest<AuthSession>("/api/auth/login", {
    method: "POST",
    body: request,
    auth: false,
  });
}
