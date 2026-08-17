export type Notificacion = {
  id: string;
  tipo: string;
  titulo: string;
  mensaje: string;
  entidadTipo: string | null;
  entidadId: string | null;
  leida: boolean;
  leidaAt: string | null;
  createdAt: string;
};

export type NotificacionResumen = {
  noLeidas: number;
};
