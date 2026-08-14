# Plan de implementación — Frontend MVP

Implementación incremental. Cada fase se detiene para revisión visual y de código antes de continuar.

---

## Fase 0 — Fundación visual ✅

**Objetivo:** Proyecto React listo, identidad visual y estructura de navegación.

| Entregable | Estado |
|------------|--------|
| Vite + React + TypeScript | ✅ |
| Layout (sidebar + contenido) | ✅ |
| Design tokens y componentes base | ✅ |
| Rutas placeholder por módulo | ✅ |
| Pantalla de acceso visual | ✅ |
| Documentación de fases | ✅ |

**Criterio de done:** `npm run dev` muestra el entorno visual; no hay llamadas al API.

---

## Fase 1 — Autenticación ✅

**Objetivo:** Login real, sesión por rol y contraseñas.

| Entregable | Estado |
|------------|--------|
| Cliente HTTP tipado | ✅ |
| Login email + contraseña | ✅ |
| Cambio obligatorio de clave | ✅ |
| Sesión JWT en localStorage | ✅ |
| Rutas protegidas | ✅ |
| Navegación por rol | ✅ |
| Gestión de usuarios (Admin) | ✅ |
| Topbar con usuario y logout | ✅ |

**Criterio de done:** Admin crea usuarios; empleado cambia clave en primer ingreso.

**Contraseñas demo:** Admin `Admin123!` · demás usuarios `Cambiar123!`

---

## Fase 2 — Legalizaciones del empleado ✅

**Objetivo:** Flujo diario del empleado.

| Entregable | Estado |
|------------|--------|
| Catálogos (monedas y categorías) | ✅ |
| Listado mis legalizaciones | ✅ |
| Crear / editar borrador | ✅ |
| Detalle con gastos y totales | ✅ |

**Criterio de done:** El empleado crea una legalización y agrega un gasto.

---

## Fase 3 — Workflow y bandejas (siguiente)

**Objetivo:** Aprobación y cierre.

| Tarea | Detalle |
|-------|---------|
| Acciones de estado | Enviar, aprobar, rechazar, reabrir, cerrar |
| Bandeja jefe | Pendientes de aprobación |
| Bandeja nómina | Pendientes de cierre |
| Historial | Línea de tiempo de estados |

**Criterio de done:** Flujo BORRADOR → CERRADA usable en UI.

---

## Fase 4 — Soportes y OCR

**Objetivo:** Adjuntar facturas y completar el gasto.

| Tarea | Detalle |
|-------|---------|
| Upload multipart | JPG, PNG, PDF |
| Procesar OCR | Campos extraídos |
| Validar / aplicar | Copiar al gasto |

**Criterio de done:** El empleado sube un soporte y aplica datos al gasto.

---

## Fase 5 — Pulido UX

**Objetivo:** Experiencia de uso agradable y estable.

| Tarea | Detalle |
|-------|---------|
| Estados vacíos y de carga | Feedback claro |
| Errores de API | Mensajes entendibles |
| Accesibilidad básica | Foco, labels, contraste |
| Ajustes visuales | Densidad, mobile |

**Criterio de done:** El MVP se puede usar de punta a punta sin fricción obvia.

---

## Diagrama de fases

```
Fase 0 ──► Fase 1 ──► Fase 2 ──► Fase 3 ──► Fase 4 ──► Fase 5
 visual     auth       empleado    workflow    docs       pulido
```
