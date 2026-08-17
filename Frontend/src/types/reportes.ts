import type { EstadoLegalizacion } from "@/types/legalizacion";
import type { UserRole } from "@/types/auth";

export type ReporteTipo =
  | "resumen-por-estado"
  | "legalizaciones-detalle"
  | "gastos-por-categoria"
  | "gastos-detalle"
  | "resumen-financiero-empleado"
  | "pendientes-aprobacion"
  | "pendientes-nomina"
  | "legalizaciones-cerradas"
  | "gastos-sin-soporte"
  | "historial-auditoria"
  | "volumen-mensual"
  | "tiempos-por-estado";

export type ReporteFiltros = {
  desde?: string;
  hasta?: string;
  departamento?: string;
  estado?: EstadoLegalizacion;
  anio?: number;
  soloCerradas?: boolean;
};

export type ResumenPorEstado = {
  estado: EstadoLegalizacion;
  cantidad: number;
  totalAnticipos: number;
  totalGastos: number;
  totalReembolsos: number;
  totalDevoluciones: number;
};

export type LegalizacionDetalleReporte = {
  id: string;
  numero: string;
  empleadoCodigo: string;
  empleadoNombre: string;
  departamento: string | null;
  jefeNombre: string | null;
  motivo: string;
  destino: string | null;
  fechaInicio: string;
  fechaFin: string;
  monedaCodigo: string;
  monedaSimbolo: string | null;
  montoAnticipo: number;
  estado: EstadoLegalizacion;
  totalGastos: number;
  totalReembolso: number;
  totalDevolucion: number;
  saldoAnticipo: number;
  createdAt: string;
  submittedAt: string | null;
  closedAt: string | null;
};

export type GastoPorCategoria = {
  categoriaCodigo: string;
  categoriaNombre: string;
  cantidadGastos: number;
  totalMonto: number;
  promedioMonto: number;
};

export type GastoDetalleReporte = {
  legalizacionNumero: string;
  legalizacionEstado: EstadoLegalizacion;
  empleadoCodigo: string;
  empleadoNombre: string;
  departamento: string | null;
  monedaCodigo: string;
  gastoId: string;
  categoriaCodigo: string;
  categoriaNombre: string;
  fechaGasto: string;
  descripcion: string;
  proveedor: string | null;
  numeroDocumento: string | null;
  monto: number;
  validado: boolean;
  cantidadSoportes: number;
};

export type ResumenFinancieroEmpleado = {
  empleadoId: string;
  empleadoCodigo: string;
  empleadoNombre: string;
  departamento: string | null;
  monedaCodigo: string;
  cantidadLegalizaciones: number;
  totalAnticipos: number;
  totalGastos: number;
  totalReembolsos: number;
  totalDevoluciones: number;
};

export type PendienteAprobacionReporte = {
  id: string;
  numero: string;
  empleadoCodigo: string;
  empleadoNombre: string;
  departamento: string | null;
  motivo: string;
  destino: string | null;
  fechaInicio: string;
  fechaFin: string;
  monedaCodigo: string;
  montoAnticipo: number;
  totalGastos: number;
  submittedAt: string | null;
  diasPendientes: number | null;
};

export type PendienteNominaReporte = {
  id: string;
  numero: string;
  empleadoCodigo: string;
  empleadoNombre: string;
  departamento: string | null;
  motivo: string;
  monedaCodigo: string;
  montoAnticipo: number;
  totalGastos: number;
  totalReembolso: number;
  totalDevolucion: number;
  submittedAt: string | null;
};

export type LegalizacionCerradaReporte = {
  id: string;
  numero: string;
  empleadoCodigo: string;
  empleadoNombre: string;
  departamento: string | null;
  monedaCodigo: string;
  montoAnticipo: number;
  totalGastos: number;
  totalReembolso: number;
  totalDevolucion: number;
  closedAt: string | null;
};

export type GastoSinSoporte = {
  legalizacionNumero: string;
  legalizacionEstado: EstadoLegalizacion;
  empleadoNombre: string;
  categoriaNombre: string;
  fechaGasto: string;
  descripcion: string;
  monto: number;
  requiereSoporte: boolean;
};

export type HistorialAuditoria = {
  historialId: string;
  legalizacionNumero: string;
  empleadoNombre: string;
  estadoAnterior: EstadoLegalizacion | null;
  estadoNuevo: EstadoLegalizacion;
  usuarioNombre: string;
  comentario: string | null;
  createdAt: string;
};

export type VolumenMensual = {
  anio: number;
  mes: number;
  periodo: string;
  cantidadLegalizaciones: number;
  totalAnticipos: number;
  totalGastos: number;
  totalReembolsos: number;
  totalDevoluciones: number;
  cantidadCerradas: number;
};

export type TiempoPorEstado = {
  legalizacionNumero: string;
  empleadoNombre: string;
  estado: EstadoLegalizacion;
  inicioEstado: string;
  finEstado: string | null;
  horasEnEstado: number;
};

export type ReporteDefinition = {
  id: ReporteTipo;
  label: string;
  description: string;
  roles: readonly UserRole[];
  supportsDateFilter: boolean;
  supportsDepartamento: boolean;
  supportsEstado: boolean;
  supportsAnio: boolean;
  supportsSoloCerradas: boolean;
};
