import { ApiError } from "@/types/auth";

const codeMessages: Record<string, string> = {
  UNAUTHORIZED: "Tu sesión expiró. Vuelve a iniciar sesión.",
  MUST_CHANGE_PASSWORD: "Debes cambiar tu contraseña antes de continuar.",
  FORBIDDEN: "No tienes permiso para realizar esta acción.",
  NOT_FOUND: "El recurso solicitado no existe o ya no está disponible.",
  INVALID_CREDENTIALS: "Correo o contraseña incorrectos.",
  OCR_ESTADO_INVALIDO: "El soporte OCR no está listo para esta acción.",
  OCR_ERROR: "No se pudo procesar el documento.",
  NO_EDITABLE: "La legalización ya no se puede modificar en este estado.",
  GASTO_NOT_FOUND: "No se encontró el gasto indicado.",
  ARCHIVO_REQUERIDO: "Debes seleccionar un archivo para continuar.",
};

export function getApiErrorMessage(
  error: unknown,
  fallback = "Ocurrió un error inesperado. Intenta de nuevo.",
): string {
  if (error instanceof ApiError) {
    if (codeMessages[error.code]) {
      return codeMessages[error.code];
    }

    if (error.message) {
      return error.message;
    }

    if (error.status === 401) return codeMessages.UNAUTHORIZED;
    if (error.status === 403) return codeMessages.FORBIDDEN;
    if (error.status === 404) return codeMessages.NOT_FOUND;
    if (error.status >= 500) return "El servidor no está disponible. Intenta más tarde.";
  }

  if (error instanceof TypeError) {
    return "No se pudo conectar con el servidor. Verifica tu conexión.";
  }

  return fallback;
}
