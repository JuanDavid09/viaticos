# Frontend — Viáticos

SPA React + TypeScript para el MVP de legalización de viáticos.

## Fase actual

**Fase 3** — Workflow y bandejas: acciones de estado, bandejas de jefe/nómina e historial.

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

## Legalizaciones (Fase 2–3)

- `/legalizaciones` — listado del empleado
- `/legalizaciones/nueva` — crear borrador
- `/legalizaciones/:id` — detalle, gastos, acciones de flujo e historial
- `/bandejas` — pendientes de aprobación (jefe) y de cierre (nómina)

### Flujo demo sugerido

1. **empleado@empresa.com** — crear borrador, agregar gasto, enviar a validación y a aprobación
2. **jefe@empresa.com** — bandejas → aprobar o rechazar con comentario
3. **empleado** — si fue aprobada, enviar a nómina; si fue rechazada, reabrir
4. **nomina@empresa.com** — bandejas → cerrar legalización

## Convención

- Alias `@/` → `src/`
- Español en la interfaz
- Cada fase se revisa antes de pasar a la siguiente
