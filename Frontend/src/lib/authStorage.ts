import type { AuthSession } from "@/types/auth";

const AUTH_STORAGE_KEY = "viaticos.auth";

export function loadSession(): AuthSession | null {
  const raw = localStorage.getItem(AUTH_STORAGE_KEY);
  if (!raw) return null;

  try {
    const session = JSON.parse(raw) as AuthSession;
    if (!session.accessToken || !session.expiresAt) return null;

    if (new Date(session.expiresAt) <= new Date()) {
      localStorage.removeItem(AUTH_STORAGE_KEY);
      return null;
    }

    return {
      ...session,
      mustChangePassword: session.mustChangePassword ?? false,
    };
  } catch {
    localStorage.removeItem(AUTH_STORAGE_KEY);
    return null;
  }
}

export function saveSession(session: AuthSession): void {
  localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(session));
}

export function clearSession(): void {
  localStorage.removeItem(AUTH_STORAGE_KEY);
}

export function getAccessToken(): string | null {
  return loadSession()?.accessToken ?? null;
}
