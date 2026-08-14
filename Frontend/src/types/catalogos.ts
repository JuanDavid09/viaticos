export type Moneda = {
  id: string;
  codigoIso: string;
  nombre: string;
  simbolo: string | null;
};

export type CategoriaGasto = {
  id: string;
  codigo: string;
  nombre: string;
  requiereSoporte: boolean;
};

export type Catalogos = {
  monedas: Moneda[];
  categorias: CategoriaGasto[];
};
