import type { LegalizacionResumen } from "@/types/legalizacion";

export type EmployeeDashboardStats = {
  total: number;
  borradores: number;
  enProceso: number;
  rechazadas: number;
  cerradas: number;
};

export type EmployeePendingItem = {
  id: string;
  numero: string;
  motivo: string;
  estado: LegalizacionResumen["estado"];
  actionLabel: string;
  actionHint: string;
};

const EN_PROCESO = new Set<LegalizacionResumen["estado"]>([
  "PendienteValidacion",
  "PendienteAprobacion",
  "Aprobada",
  "PendienteNomina",
]);

function getPendingMeta(item: LegalizacionResumen): Pick<EmployeePendingItem, "actionLabel" | "actionHint"> | null {
  switch (item.estado) {
    case "Borrador":
      return {
        actionLabel: "Continuar borrador",
        actionHint: "Completa los gastos y envía la legalización.",
      };
    case "PendienteValidacion":
      return {
        actionLabel: "Enviar a aprobación",
        actionHint: "Revisa los datos y envía al flujo de aprobación.",
      };
    case "Rechazada":
      return {
        actionLabel: "Revisar rechazo",
        actionHint: "Corrige la legalización y reábrela como borrador.",
      };
    case "Aprobada":
      return {
        actionLabel: "Enviar a nómina",
        actionHint: "La legalización fue aprobada y puede enviarse a nómina.",
      };
    default:
      return null;
  }
}

export function buildEmployeeDashboard(items: LegalizacionResumen[]) {
  const stats: EmployeeDashboardStats = {
    total: items.length,
    borradores: items.filter((item) => item.estado === "Borrador").length,
    enProceso: items.filter((item) => EN_PROCESO.has(item.estado)).length,
    rechazadas: items.filter((item) => item.estado === "Rechazada").length,
    cerradas: items.filter((item) => item.estado === "Cerrada").length,
  };

  const pendientes = items
    .map((item) => {
      const meta = getPendingMeta(item);
      if (!meta) return null;
      return {
        id: item.id,
        numero: item.numero,
        motivo: item.motivo,
        estado: item.estado,
        ...meta,
      };
    })
    .filter((item): item is EmployeePendingItem => item !== null)
    .sort((a, b) => a.numero.localeCompare(b.numero, undefined, { numeric: true }));

  const recientes = [...items]
    .sort((a, b) => b.createdAt.localeCompare(a.createdAt))
    .slice(0, 5);

  return { stats, pendientes, recientes };
}
