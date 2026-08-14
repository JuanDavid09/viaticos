# Frontend — Viáticos

SPA React + TypeScript para el MVP de legalización de viáticos.

## Fase actual

**Fase 1** — Autenticación con contraseña, cambio obligatorio en primer acceso y gestión de usuarios (Admin).

## Stack

| Pieza | Elección |
|-------|----------|
| Bundler | Vite 5 |
| UI | React 18 + TypeScript |
| Routing | React Router 6 |
| Iconos | lucide-react |
| Estilos | CSS con design tokens |

## Comandos

```powershell
cd Frontend
npm install
npm run dev
```

Abrir http://localhost:5173

El proxy de Vite reenvía `/api` al backend en `http://localhost:5228`. Asegúrate de tener el API en ejecución.

## Login y seguridad

- `/login` — correo + contraseña
- `/cambiar-clave` — obligatorio si `mustChangePassword` es true
- `/usuarios` — solo Admin: crear usuarios, restablecer claves, activar/desactivar

### Contraseñas demo (seed)

| Correo | Contraseña | Notas |
|--------|------------|-------|
| admin@empresa.com | Admin123! | Acceso directo al panel |
| empleado@empresa.com | Cambiar123! | Debe cambiar clave al ingresar |
| jefe@empresa.com | Cambiar123! | Debe cambiar clave al ingresar |
| nomina@empresa.com | Cambiar123! | Debe cambiar clave al ingresar |

Si la BD ya existía antes de este cambio, ejecuta `database/009_auth_password.sql`.

## Convención

- Alias `@/` → `src/`
- Español en la interfaz
- Cada fase se revisa antes de pasar a la siguiente
