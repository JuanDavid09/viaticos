import { useState, type FormEvent } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { LoginLogo } from "@/components/auth/LoginLogo";
import { PasswordInput } from "@/components/ui/PasswordInput";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";
import { env } from "@/config/env";
import { getApiErrorMessage } from "@/lib/apiErrorMessage";

export function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const redirectTo =
    (location.state as { from?: string } | null)?.from ?? appRoutes.home;

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      const session = await login(email, password);
      navigate(
        session.mustChangePassword ? appRoutes.cambiarClave : redirectTo,
        { replace: true },
      );
    } catch (err) {
      setError(getApiErrorMessage(err, "No se pudo conectar con el servidor."));
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="login-screen">
      <section className="login-brand" aria-hidden="true">
        <LoginLogo />
        <p className="login-brand-tagline">{env.appTagline}</p>
      </section>

      <section className="login-panel" aria-labelledby="login-title">
        <LoginLogo variant="compact" />

        <div className="login-panel-header">
          <h1 id="login-title" className="login-title">Iniciar sesión</h1>
          <p className="login-subtitle">
            Accede con tu correo corporativo. Si es tu primer ingreso, deberás definir
            una contraseña personal.
          </p>
        </div>

        <form className="login-form" onSubmit={handleSubmit}>
          <label htmlFor="email">Correo corporativo</label>
          <input
            id="email"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            placeholder="nombre.apellido@empresa.com"
            autoComplete="username"
            disabled={isSubmitting}
            required
          />

          <label htmlFor="password">Contraseña</label>
          <PasswordInput
            id="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            autoComplete="current-password"
            disabled={isSubmitting}
            required
          />

          {error ? (
            <p className="login-error" role="alert">
              {error}
            </p>
          ) : null}

          <button className="btn btn-primary login-submit" type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Ingresando…" : "Ingresar"}
          </button>
        </form>

        <p className="login-footer">Acceso restringido a personal autorizado.</p>
      </section>
    </div>
  );
}
