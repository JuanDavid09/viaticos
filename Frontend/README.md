# Frontend — Viáticos

SPA React + TypeScript para el MVP de legalización de viáticos.

## Fase actual

**Fase 0** — Fundación visual. Sin conexión al API.

## Stack

| Pieza | Elección |
|-------|----------|
| Bundler | Vite 5 |
| UI | React 18 + TypeScript |
| Routing | React Router 6 |
| Iconos | lucide-react |
| Estilos | CSS con design tokens |

## Estructura

```
Frontend/
├── docs/PHASES.md
├── public/
├── src/
│   ├── app/              # Router y rutas
│   ├── components/layout # Shell, sidebar, topbar
│   ├── config/           # Variables de entorno
│   ├── pages/            # Pantallas
│   └── styles/           # Tokens y estilos globales
└── package.json
```

## Comandos

```powershell
cd Frontend
npm install
npm run dev
```

Abrir http://localhost:5173

- `/login` — pantalla de acceso visual
- `/` — panel inicial
- `/legalizaciones`, `/bandejas`, `/soportes` — placeholders de módulos

## Convención

- Alias `@/` → `src/`
- Español en la interfaz
- Cada fase se revisa antes de pasar a la siguiente
