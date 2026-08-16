# Frontend — Viáticos

SPA React + TypeScript para el MVP de legalización de viáticos.

## Fase actual

**Fase 5** — Pulido UX: estados de carga, errores con reintento, confirmaciones, accesibilidad y navegación móvil.

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

## Legalizaciones y soportes (Fase 2–4)

- `/legalizaciones` — listado del empleado
- `/legalizaciones/nueva` — crear borrador
- `/legalizaciones/:id` — detalle, gastos, workflow, historial y soportes/OCR por gasto
- `/bandejas` — pendientes de aprobación (jefe) y de cierre (nómina)
- `/soportes` — acceso rápido a legalizaciones editables para adjuntar facturas

### Soportes y OCR

1. En el detalle de una legalización editable, expande un gasto y adjunta JPG, PNG o PDF (máx. 10 MB).
2. El sistema procesa OCR automáticamente (mock en desarrollo).
3. Revisa o corrige los campos extraídos y pulsa **Validar campos**.
4. Pulsa **Aplicar al gasto** para copiar proveedor, documento, monto y fecha.

### Flujo demo sugerido

1. **empleado@empresa.com** — crear borrador, agregar gasto, enviar a validación y a aprobación
2. **jefe@empresa.com** — bandejas → aprobar o rechazar con comentario
3. **empleado** — si fue aprobada, enviar a nómina; si fue rechazada, reabrir
4. **nomina@empresa.com** — bandejas → cerrar legalización

## Convención

- Alias `@/` → `src/`
- Español en la interfaz
- Cada fase se revisa antes de pasar a la siguiente
