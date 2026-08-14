import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { login as loginRequest } from "@/api/auth";
import { clearSession, loadSession, saveSession } from "@/lib/authStorage";
import type { AuthSession, UserRole } from "@/types/auth";
import { hasAnyRole } from "@/features/auth/roleUtils";

type AuthContextValue = {
  session: AuthSession | null;
  isAuthenticated: boolean;
  isBootstrapping: boolean;
  login: (email: string) => Promise<void>;
  logout: () => void;
  hasRole: (...roles: UserRole[]) => boolean;
};

const AuthContext = createContext<AuthContextValue | null>(null);

type AuthProviderProps = {
  children: ReactNode;
};

export function AuthProvider({ children }: AuthProviderProps) {
  const [session, setSession] = useState<AuthSession | null>(() => loadSession());
  const [isBootstrapping] = useState(false);

  const login = useCallback(async (email: string) => {
    const nextSession = await loginRequest({ email: email.trim() });
    saveSession(nextSession);
    setSession(nextSession);
  }, []);

  const logout = useCallback(() => {
    clearSession();
    setSession(null);
  }, []);

  const hasRole = useCallback(
    (...roles: UserRole[]) => {
      if (!session) return false;
      return hasAnyRole(session.rol, roles);
    },
    [session],
  );

  const value = useMemo<AuthContextValue>(
    () => ({
      session,
      isAuthenticated: session !== null,
      isBootstrapping,
      login,
      logout,
      hasRole,
    }),
    [session, isBootstrapping, login, logout, hasRole],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth debe usarse dentro de AuthProvider.");
  }
  return context;
}
