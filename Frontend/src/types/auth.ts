export type UserRole = "EMPLEADO" | "JEFE_APROBADOR" | "NOMINA" | "ADMIN";

export type AuthSession = {
  accessToken: string;
  expiresAt: string;
  userId: string;
  email: string;
  rol: UserRole;
  nombreCompleto: string;
  mustChangePassword: boolean;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type ChangePasswordRequest = {
  currentPassword: string;
  newPassword: string;
};

export type ApiErrorBody = {
  code?: string;
  message?: string;
};

export class ApiError extends Error {
  readonly code: string;
  readonly status: number;

  constructor(status: number, code: string, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.code = code;
  }
}

export function mapLoginResponse(response: Omit<AuthSession, "mustChangePassword"> & {
  mustChangePassword?: boolean;
}): AuthSession {
  return {
    ...response,
    mustChangePassword: response.mustChangePassword ?? false,
  };
}
