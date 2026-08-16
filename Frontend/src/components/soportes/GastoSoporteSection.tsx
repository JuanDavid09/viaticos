import { useState } from "react";
import { AlertCircle } from "lucide-react";
import { SoporteOcrPanel } from "@/components/soportes/SoporteOcrPanel";
import { SoporteUploadForm } from "@/components/soportes/SoporteUploadForm";
import { subirSoporte, procesarOcr } from "@/api/soportes";
import { getLegalizacion } from "@/api/legalizaciones";
import { formatFileSize } from "@/features/soportes/ocrUtils";
import { ApiError } from "@/types/auth";
import type { Gasto, LegalizacionDetalle } from "@/types/legalizacion";

type GastoSoporteSectionProps = {
  legalizacionId: string;
  gasto: Gasto;
  editable: boolean;
  requiereSoporte: boolean;
  onLegalizacionUpdated: (legalizacion: LegalizacionDetalle) => void;
  onMessage: (message: string | null) => void;
  onError: (message: string | null) => void;
};

export function GastoSoporteSection({
  legalizacionId,
  gasto,
  editable,
  requiereSoporte,
  onLegalizacionUpdated,
  onMessage,
  onError,
}: GastoSoporteSectionProps) {
  const [isUploading, setIsUploading] = useState(false);
  const soportes = gasto.soportes ?? [];
  const missingSoporte = requiereSoporte && soportes.length === 0;

  async function handleUpload(file: File, esPrincipal: boolean) {
    setIsUploading(true);
    onError(null);
    onMessage(null);
    try {
      const uploaded = await subirSoporte(legalizacionId, gasto.id, file, esPrincipal);
      await procesarOcr(uploaded.gastoSoporteId);
      onMessage(`Soporte "${uploaded.nombreOriginal}" subido y OCR procesado.`);
      onLegalizacionUpdated(await getLegalizacion(legalizacionId));
    } catch (err) {
      onError(err instanceof ApiError ? err.message : "No se pudo subir el soporte.");
    } finally {
      setIsUploading(false);
    }
  }

  return (
    <div className="gasto-soporte-section">
      <div className="gasto-soporte-header">
        <span className="table-meta">
          Soportes ({soportes.length})
          {soportes.length > 0
            ? ` · ${formatFileSize(soportes.reduce((total, item) => total + item.tamanoBytes, 0))} total`
            : ""}
        </span>
        {missingSoporte ? (
          <span className="soporte-warning">
            <AlertCircle size={14} />
            Requiere soporte
          </span>
        ) : null}
      </div>

      {soportes.length === 0 && !editable ? (
        <p className="table-meta">Sin soportes adjuntos.</p>
      ) : null}

      {soportes.map((soporte) => (
        <SoporteOcrPanel
          key={soporte.id}
          soporte={soporte}
          editable={editable}
          onLegalizacionUpdated={onLegalizacionUpdated}
          onMessage={onMessage}
          onError={onError}
        />
      ))}

      {editable ? (
        <SoporteUploadForm isSubmitting={isUploading} onUpload={handleUpload} />
      ) : null}
    </div>
  );
}
