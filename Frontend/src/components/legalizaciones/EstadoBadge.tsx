import type { EstadoLegalizacion } from "@/types/legalizacion";
import { getEstadoLabel, getEstadoTone } from "@/features/legalizaciones/legalizacionUtils";

type EstadoBadgeProps = {
  estado: EstadoLegalizacion;
};

export function EstadoBadge({ estado }: EstadoBadgeProps) {
  return (
    <span className={`status-badge status-${getEstadoTone(estado)}`}>
      {getEstadoLabel(estado)}
    </span>
  );
}
