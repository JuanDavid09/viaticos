import { useEffect, useState, type FormEvent } from "react";
import { X } from "lucide-react";
import { PasswordInput } from "@/components/ui/PasswordInput";

type ResetPasswordModalProps = {
  isOpen: boolean;
  empleadoNombre: string;
  isSubmitting: boolean;
  onClose: () => void;
  onSubmit: (passwordTemporal: string) => void;
};

export function ResetPasswordModal({
  isOpen,
  empleadoNombre,
  isSubmitting,
  onClose,
  onSubmit,
}: ResetPasswordModalProps) {
  const [passwordTemporal, setPasswordTemporal] = useState("Cambiar123!");

  useEffect(() => {
    if (isOpen) {
      setPasswordTemporal("Cambiar123!");
    }
  }, [isOpen]);

  if (!isOpen) return null;

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    onSubmit(passwordTemporal);
  }

  return (
    <div className="modal-backdrop" role="presentation" onClick={onClose}>
      <div
        className="modal-card"
        role="dialog"
        aria-modal="true"
        aria-labelledby="reset-password-title"
        onClick={(event) => event.stopPropagation()}
      >
        <div className="modal-header">
          <h3 id="reset-password-title">Restablecer contraseña</h3>
          <button type="button" className="btn btn-ghost modal-close" onClick={onClose} aria-label="Cerrar">
            <X size={18} />
          </button>
        </div>

        <p className="table-meta">Nueva contraseña temporal para {empleadoNombre}.</p>

        <form className="stack-form" onSubmit={handleSubmit}>
          <label htmlFor="reset-password-input">
            Contraseña temporal
            <PasswordInput
              id="reset-password-input"
              value={passwordTemporal}
              onChange={(event) => setPasswordTemporal(event.target.value)}
              required
              minLength={8}
            />
          </label>

          <div className="modal-actions">
            <button type="button" className="btn btn-ghost" onClick={onClose} disabled={isSubmitting}>
              Cancelar
            </button>
            <button type="submit" className="btn btn-primary" disabled={isSubmitting || !passwordTemporal.trim()}>
              {isSubmitting ? "Guardando…" : "Restablecer"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
