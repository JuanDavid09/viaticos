import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { changePassword as changePasswordRequest, login as loginRequest } from "@/api/auth";
import { clearSession, loadSession, saveSession } from "@/lib/authStorage";
import type { AuthSession, ChangePasswordRequest, UserRole } from "@/types/auth";
import { ApiError } from "@/types/auth";
import { hasAnyRole } from "@/features/auth/roleUtils";

type AuthContextValue = {
  session: AuthSession | null;
  isAuthenticated: boolean;
  isBootstrapping: boolean;
  mustChangePassword: boolean;
  login: (email: string, password: string) => Promise<AuthSession>;
  changePassword: (request: ChangePasswordRequest) => Promise<void>;
  applySession: (session: AuthSession) => void;
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

  const applySession = useCallback((nextSession: AuthSession) => {
    saveSession(nextSession);
    setSession(nextSession);
  }, []);

  const login = useCallback(async (email: string, password: string) => {
    const nextSession = await loginRequest({ email: email.trim(), password });
    applySession(nextSession);
    return nextSession;
  }, [applySession]);

  const changePassword = useCallback(
    async (request: ChangePasswordRequest) => {
      if (!session?.accessToken) {
        throw new ApiError(401, "UNAUTHORIZED", "Sesión no válida. Vuelve a iniciar sesión.");
      }
      const nextSession = await changePasswordRequest(request, session.accessToken);
      applySession(nextSession);
    },
    [session, applySession],
  );

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
      mustChangePassword: session?.mustChangePassword ?? false,
      login,
      changePassword,
      applySession,
      logout,
      hasRole,
    }),
    [session, isBootstrapping, login, changePassword, applySession, logout, hasRole],
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
