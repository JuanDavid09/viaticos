import type { EstadoOcr } from "@/types/soporte";
import { getOcrEstadoLabel, getOcrEstadoTone } from "@/features/soportes/ocrUtils";

type OcrEstadoBadgeProps = {
  estado: EstadoOcr | null | undefined;
};

export function OcrEstadoBadge({ estado }: OcrEstadoBadgeProps) {
  return (
    <span className={`status-badge status-${getOcrEstadoTone(estado)}`}>
      OCR: {getOcrEstadoLabel(estado)}
    </span>
  );
}
