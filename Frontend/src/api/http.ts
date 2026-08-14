import { env } from "@/config/env";
import { getAccessToken } from "@/lib/authStorage";
import { ApiError, type ApiErrorBody } from "@/types/auth";

type RequestOptions = Omit<RequestInit, "body"> & {
  body?: unknown;
  auth?: boolean;
  accessToken?: string | null;
};

function buildUrl(path: string): string {
  const base = env.apiBaseUrl.replace(/\/$/, "");
  const normalizedPath = path.startsWith("/") ? path : `/${path}`;
  return `${base}${normalizedPath}`;
}

async function parseError(response: Response): Promise<ApiError> {
  try {
    const body = (await response.json()) as ApiErrorBody;
    return new ApiError(
      response.status,
      body.code ?? "UNKNOWN_ERROR",
      body.message ?? "Ocurrió un error inesperado.",
    );
  } catch {
    return new ApiError(response.status, "HTTP_ERROR", response.statusText || "Error de red.");
  }
}

export async function apiRequest<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { body, auth = true, accessToken, headers, ...rest } = options;

  const requestHeaders = new Headers(headers);
  if (body !== undefined) {
    requestHeaders.set("Content-Type", "application/json");
  }

  if (auth) {
    const token = accessToken ?? getAccessToken();
    if (!token) {
      throw new ApiError(401, "UNAUTHORIZED", "Sesión no válida. Vuelve a iniciar sesión.");
    }
    requestHeaders.set("Authorization", `Bearer ${token}`);
  }

  const response = await fetch(buildUrl(path), {
    ...rest,
    headers: requestHeaders,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    throw await parseError(response);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}
