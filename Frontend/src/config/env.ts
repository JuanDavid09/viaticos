export const env = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? "",
  /** URL absoluta o ruta en /public (ej. /logo.svg). Dejar vacío para usar el logo por defecto. */
  logoUrl: import.meta.env.VITE_APP_LOGO_URL ?? "/logo.svg",
  /** Color principal de marca (hex). Alinea botones y acentos con el logotipo. */
  brandColor: import.meta.env.VITE_APP_BRAND_COLOR ?? "",
  appName: import.meta.env.VITE_APP_NAME ?? "Viáticos",
  appTagline:
    import.meta.env.VITE_APP_TAGLINE ??
    "Plataforma interna de legalización y aprobación de gastos de viaje.",
} as const;
