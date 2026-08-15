export type EstadoLegalizacion =
  | "Borrador"
  | "PendienteValidacion"
  | "PendienteAprobacion"
  | "Aprobada"
  | "Rechazada"
  | "PendienteNomina"
  | "Cerrada";

export type LegalizacionResumen = {
  id: string;
  numero: string;
  motivo: string;
  destino: string | null;
  fechaInicio: string;
  fechaFin: string;
  estado: EstadoLegalizacion;
  totalGastos: number;
  totalReembolso: number;
  totalDevolucion: number;
  createdAt: string;
};

export type Gasto = {
  id: string;
  categoriaGastoId: string;
  fechaGasto: string;
  descripcion: string;
  proveedor: string | null;
  numeroDocumento: string | null;
  monto: number;
  validado: boolean;
  orden: number;
  soportes: unknown[];
};

export type LegalizacionDetalle = {
  id: string;
  numero: string;
  empleadoId: string;
  motivo: string;
  destino: string | null;
  fechaInicio: string;
  fechaFin: string;
  monedaId: string;
  montoAnticipo: number;
  estado: EstadoLegalizacion;
  totalGastos: number;
  totalReembolso: number;
  totalDevolucion: number;
  observaciones: string | null;
  gastos: Gasto[];
};

export type CrearLegalizacionRequest = {
  motivo: string;
  fechaInicio: string;
  fechaFin: string;
  monedaId: string;
  montoAnticipo: number;
  destino?: string;
};

export type ActualizarLegalizacionRequest = CrearLegalizacionRequest;

export type AgregarGastoRequest = {
  categoriaGastoId: string;
  fechaGasto: string;
  descripcion: string;
  monto: number;
  proveedor?: string;
  numeroDocumento?: string;
};

export type RechazarLegalizacionRequest = {
  comentario: string;
};

export type LegalizacionHistorial = {
  id: string;
  estadoAnterior: EstadoLegalizacion | null;
  estadoNuevo: EstadoLegalizacion;
  usuarioId: string;
  comentario: string | null;
  createdAt: string;
};

export type WorkflowAction =
  | "enviar-validacion"
  | "enviar-aprobacion"
  | "aprobar"
  | "rechazar"
  | "reabrir"
  | "enviar-nomina"
  | "cerrar";

export type LegalizacionFormValues = {
  motivo: string;
  destino: string;
  fechaInicio: string;
  fechaFin: string;
  monedaId: string;
  montoAnticipo: string;
};

export type GastoFormValues = {
  categoriaGastoId: string;
  fechaGasto: string;
  descripcion: string;
  monto: string;
  proveedor: string;
  numeroDocumento: string;
};

export const emptyLegalizacionForm: LegalizacionFormValues = {
  motivo: "",
  destino: "",
  fechaInicio: "",
  fechaFin: "",
  monedaId: "",
  montoAnticipo: "0",
};

export const emptyGastoForm: GastoFormValues = {
  categoriaGastoId: "",
  fechaGasto: "",
  descripcion: "",
  monto: "",
  proveedor: "",
  numeroDocumento: "",
};
