import { useCallback, useEffect, useState } from "react";
import { getCatalogos } from "@/api/catalogos";
import { ApiError } from "@/types/auth";
import type { Catalogos } from "@/types/catalogos";

let cachedCatalogos: Catalogos | null = null;
let inflight: Promise<Catalogos> | null = null;

export function useCatalogos() {
  const [catalogos, setCatalogos] = useState<Catalogos | null>(cachedCatalogos);
  const [isLoading, setIsLoading] = useState(!cachedCatalogos);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async (force = false) => {
    if (!force && cachedCatalogos) {
      setCatalogos(cachedCatalogos);
      setIsLoading(false);
      return cachedCatalogos;
    }

    if (!force && inflight) {
      return inflight;
    }

    setIsLoading(true);
    setError(null);

    inflight = getCatalogos()
      .then((data) => {
        cachedCatalogos = data;
        setCatalogos(data);
        return data;
      })
      .catch((err) => {
        const message =
          err instanceof ApiError ? err.message : "No se pudieron cargar los catálogos.";
        setError(message);
        throw err;
      })
      .finally(() => {
        setIsLoading(false);
        inflight = null;
      });

    return inflight;
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  return { catalogos, isLoading, error, reload: () => load(true) };
}

export function findMoneda(catalogos: Catalogos | null, monedaId: string) {
  return catalogos?.monedas.find((item) => item.id === monedaId) ?? null;
}

export function findCategoria(catalogos: Catalogos | null, categoriaId: string) {
  return catalogos?.categorias.find((item) => item.id === categoriaId) ?? null;
}
