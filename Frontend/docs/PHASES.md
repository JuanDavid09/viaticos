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

## Fase 1 — Autenticación (siguiente)

**Objetivo:** Login real y sesión por rol.

| Tarea | Detalle |
|-------|---------|
| Cliente HTTP | `fetch` tipado contra `/api` |
| Login | `POST /api/auth/login` |
| Sesión | Token JWT, usuario y rol |
| Rutas protegidas | Redirección a login |
| Navegación por rol | Empleado / Jefe / Nómina / Admin |

**Criterio de done:** Un usuario seed entra y ve el shell autenticado.

---

## Fase 2 — Legalizaciones del empleado

**Objetivo:** Flujo diario del empleado.

| Tarea | Detalle |
|-------|---------|
| Catálogos | Monedas y categorías |
| Listado | Mis legalizaciones |
| Crear / editar | Borrador |
| Detalle | Gastos y totales |

**Criterio de done:** El empleado crea una legalización y agrega un gasto.

---

## Fase 3 — Workflow y bandejas

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
