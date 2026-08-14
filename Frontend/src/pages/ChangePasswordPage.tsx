import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { appRoutes } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";
import { ApiError } from "@/types/auth";

export function ChangePasswordPage() {
  const navigate = useNavigate();
  const { session, changePassword, logout } = useAuth();
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (newPassword !== confirmPassword) {
      setError("La confirmación no coincide con la nueva contraseña.");
      return;
    }

    setIsSubmitting(true);

    try {
      await changePassword({ currentPassword, newPassword });
      navigate(appRoutes.home, { replace: true });
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.message);
      } else {
        setError("No se pudo actualizar la contraseña.");
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="login-screen">
      <section className="login-panel">
        <p className="page-kicker">Seguridad de la cuenta</p>
        <h1 className="page-title">Cambiar contraseña</h1>
        <p className="page-lead">
          {session
            ? `${session.nombreCompleto}, debes definir una contraseña personal antes de usar el sistema.`
            : "Define tu contraseña personal para continuar."}
        </p>

        <form className="login-form" onSubmit={handleSubmit}>
          <label htmlFor="current-password">Contraseña actual</label>
          <input
            id="current-password"
            type="password"
            value={currentPassword}
            onChange={(event) => setCurrentPassword(event.target.value)}
            autoComplete="current-password"
            disabled={isSubmitting}
            required
          />

          <label htmlFor="new-password">Nueva contraseña</label>
          <input
            id="new-password"
            type="password"
            value={newPassword}
            onChange={(event) => setNewPassword(event.target.value)}
            autoComplete="new-password"
            disabled={isSubmitting}
            required
            minLength={8}
          />

          <label htmlFor="confirm-password">Confirmar nueva contraseña</label>
          <input
            id="confirm-password"
            type="password"
            value={confirmPassword}
            onChange={(event) => setConfirmPassword(event.target.value)}
            autoComplete="new-password"
            disabled={isSubmitting}
            required
            minLength={8}
          />

          <p className="login-hint">
            Mínimo 8 caracteres, con mayúscula, minúscula y número.
          </p>

          {error ? (
            <p className="login-error" role="alert">
              {error}
            </p>
          ) : null}

          <button className="btn btn-primary" type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Guardando…" : "Guardar contraseña"}
          </button>

          <button
            type="button"
            className="btn btn-ghost"
            disabled={isSubmitting}
            onClick={() => {
              logout();
              navigate(appRoutes.login, { replace: true });
            }}
          >
            Cerrar sesión
          </button>
        </form>
      </section>
    </div>
  );
}
