# Frontend — Viáticos

SPA React + TypeScript para el MVP de legalización de viáticos.

## Fase actual

**Fase 1** — Autenticación JWT, sesión por rol y rutas protegidas.

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
│   ├── api/              # Cliente HTTP y auth
│   ├── app/              # Router y rutas
│   ├── components/layout # Shell, sidebar, topbar
│   ├── config/           # Variables de entorno
│   ├── features/auth/    # Contexto, rutas protegidas, roles
│   ├── lib/              # Persistencia de sesión
│   ├── pages/            # Pantallas
│   ├── types/            # Tipos compartidos
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

El proxy de Vite reenvía `/api` al backend en `http://localhost:5228`. Asegúrate de tener el API en ejecución.

## Login (Fase 1)

- `/login` — formulario con correo corporativo (sin contraseña en el MVP)
- Sesión JWT guardada en `localStorage`
- Rutas internas redirigen a login si no hay sesión
- Menú lateral filtrado por rol

Usuarios seed:

| Correo | Rol |
|--------|-----|
| empleado@empresa.com | Empleado |
| jefe@empresa.com | Jefe aprobador |
| nomina@empresa.com | Nómina |
| admin@empresa.com | Administrador |

## Convención

- Alias `@/` → `src/`
- Español en la interfaz
- Cada fase se revisa antes de pasar a la siguiente
