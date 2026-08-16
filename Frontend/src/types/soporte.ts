export type EstadoOcr =
  | "Pendiente"
  | "Procesando"
  | "Completado"
  | "Error"
  | "ValidadoUsuario";

export type GastoSoporte = {
  id: string;
  archivoId: string;
  nombreOriginal: string;
  mimeType: string;
  tamanoBytes: number;
  esPrincipal: boolean;
  ocrExtraccionId: string | null;
  ocrEstado: EstadoOcr | null;
};

export type OcrCampo = {
  id: string;
  nombreCampo: string;
  valorExtraido: string | null;
  valorValidado: string | null;
  validado: boolean;
};

export type OcrExtraccion = {
  id: string;
  gastoSoporteId: string;
  estado: EstadoOcr;
  errorMensaje: string | null;
  procesadoAt: string | null;
  campos: OcrCampo[];
};

export type SubirSoporteResponse = {
  gastoSoporteId: string;
  archivoId: string;
  ocrExtraccionId: string;
  nombreOriginal: string;
};

export type ValidarCampoOcrRequest = {
  campoId: string;
  valorValidado: string;
};

export type ValidarCamposOcrRequest = {
  campos: ValidarCampoOcrRequest[];
};

export type OcrCampoFormValue = {
  campoId: string;
  nombreCampo: string;
  value: string;
};
