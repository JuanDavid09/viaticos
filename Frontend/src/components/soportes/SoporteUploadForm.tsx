import { useRef, useState, type FormEvent } from "react";
import { Upload } from "lucide-react";
import { ACCEPTED_SOPORTE_MIME, ACCEPTED_SOPORTE_TYPES } from "@/features/soportes/ocrUtils";

type SoporteUploadFormProps = {
  isSubmitting: boolean;
  onUpload: (file: File, esPrincipal: boolean) => Promise<void>;
};

export function SoporteUploadForm({ isSubmitting, onUpload }: SoporteUploadFormProps) {
  const inputRef = useRef<HTMLInputElement>(null);
  const [esPrincipal, setEsPrincipal] = useState(true);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    const file = inputRef.current?.files?.[0];
    if (!file) {
      setError("Selecciona un archivo JPG, PNG o PDF.");
      return;
    }

    if (!ACCEPTED_SOPORTE_MIME.includes(file.type)) {
      setError("Tipo de archivo no permitido. Use JPG, PNG o PDF.");
      return;
    }

    if (file.size > 10 * 1024 * 1024) {
      setError("El archivo no puede superar 10 MB.");
      return;
    }

    await onUpload(file, esPrincipal);
    if (inputRef.current) {
      inputRef.current.value = "";
    }
  }

  return (
    <form className="soporte-upload stack-form" onSubmit={(event) => void handleSubmit(event)}>
      <label htmlFor="soporte-file">
        Adjuntar soporte
        <input
          ref={inputRef}
          id="soporte-file"
          type="file"
          accept={ACCEPTED_SOPORTE_TYPES}
          disabled={isSubmitting}
        />
      </label>

      <label className="checkbox-row">
        <input
          type="checkbox"
          checked={esPrincipal}
          onChange={(event) => setEsPrincipal(event.target.checked)}
          disabled={isSubmitting}
        />
        Marcar como soporte principal
      </label>

      {error ? <p className="login-error" role="alert">{error}</p> : null}

      <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
        <Upload size={16} />
        {isSubmitting ? "Subiendo…" : "Subir y procesar OCR"}
      </button>
    </form>
  );
}
