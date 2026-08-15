import { useEffect, useState, type FormEvent } from "react";
import { X } from "lucide-react";

type RechazarModalProps = {
  isOpen: boolean;
  isSubmitting: boolean;
  onClose: () => void;
  onSubmit: (comentario: string) => void;
};

export function RechazarModal({ isOpen, isSubmitting, onClose, onSubmit }: RechazarModalProps) {
  const [comentario, setComentario] = useState("");

  useEffect(() => {
    if (isOpen) {
      setComentario("");
    }
  }, [isOpen]);

  if (!isOpen) return null;

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    onSubmit(comentario.trim());
  }

  return (
    <div className="modal-backdrop" role="presentation" onClick={onClose}>
      <div
        className="modal-card"
        role="dialog"
        aria-modal="true"
        aria-labelledby="rechazar-title"
        onClick={(event) => event.stopPropagation()}
      >
        <div className="modal-header">
          <h3 id="rechazar-title">Rechazar legalización</h3>
          <button type="button" className="btn btn-ghost modal-close" onClick={onClose} aria-label="Cerrar">
            <X size={18} />
          </button>
        </div>

        <form className="stack-form" onSubmit={handleSubmit}>
          <label htmlFor="rechazar-comentario">
            Comentario
            <textarea
              id="rechazar-comentario"
              value={comentario}
              onChange={(event) => setComentario(event.target.value)}
              placeholder="Indica el motivo del rechazo"
              required
              minLength={1}
              maxLength={2000}
            />
          </label>

          <div className="modal-actions">
            <button type="button" className="btn btn-ghost" onClick={onClose} disabled={isSubmitting}>
              Cancelar
            </button>
            <button type="submit" className="btn btn-danger" disabled={isSubmitting || !comentario.trim()}>
              {isSubmitting ? "Rechazando…" : "Confirmar rechazo"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
