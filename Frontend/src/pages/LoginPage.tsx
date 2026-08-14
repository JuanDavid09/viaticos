import { useState, type FormEvent } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";
import { demoUsers } from "@/features/auth/roleUtils";
import { ApiError } from "@/types/auth";

export function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();
  const [email, setEmail] = useState("empleado@empresa.com");
  const [password, setPassword] = useState("Cambiar123!");
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
      if (err instanceof ApiError) {
        setError(err.message);
      } else {
        setError("No se pudo conectar con el servidor. Verifica que el API esté en ejecución.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  function fillDemoUser(userEmail: string, demoPassword: string) {
    setEmail(userEmail);
    setPassword(demoPassword);
  }

  return (
    <div className="login-screen">
      <section className="login-panel">
        <p className="page-kicker">Acceso interno</p>
        <h1 className="page-title">Legalización de viáticos</h1>
        <p className="page-lead">
          Ingresa con tu correo y contraseña. Si es tu primer acceso, el sistema te pedirá
          definir una clave personal.
        </p>

        <form className="login-form" onSubmit={handleSubmit}>
          <label htmlFor="email">Correo corporativo</label>
          <input
            id="email"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            placeholder="empleado@empresa.com"
            autoComplete="username"
            disabled={isSubmitting}
            required
          />

          <label htmlFor="password">Contraseña</label>
          <input
            id="password"
            type="password"
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

          <button className="btn btn-primary" type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Ingresando…" : "Ingresar"}
          </button>
        </form>

        <div className="login-demo">
          <p className="login-hint">Usuarios demo del seed:</p>
          <div className="login-demo-list">
            {demoUsers.map((user) => (
              <button
                key={user.email}
                type="button"
                className="btn btn-ghost login-demo-btn"
                disabled={isSubmitting}
                onClick={() => fillDemoUser(user.email, user.password)}
              >
                {user.label}
              </button>
            ))}
          </div>
          <p className="login-hint">Admin: Admin123! · Demás usuarios: Cambiar123!</p>
        </div>
      </section>
    </div>
  );
}
