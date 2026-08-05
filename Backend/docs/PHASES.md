# Plan de implementación — Backend MVP

Implementación incremental. Cada fase es funcional y desplegable.

---

## Fase 0 — Fundamentos ✅ (actual)

**Objetivo:** Estructura del solution y documentación de arquitectura.

| Entregable | Estado |
|------------|--------|
| Solution .NET 8 con 4 proyectos + tests | ✅ |
| Documento de arquitectura | ✅ |
| Estructura de carpetas por módulo | ✅ |
| Enums de dominio alineados a BD | ✅ |
| `.gitignore` | ✅ |

**No incluye:** lógica de negocio, EF Core, endpoints.

---

## Fase 1 — Dominio + Persistencia base

**Objetivo:** Modelo de dominio y conexión a PostgreSQL.

| Tarea | Detalle |
|-------|---------|
| Base classes | `Entity`, `AggregateRoot`, `DomainException` |
| Entidades | `Legalizacion`, `Gasto`, `Empleado`, `Archivo`, catálogos |
| Máquina de estados | Métodos de transición en `Legalizacion` |
| EF Core | `ViaticosDbContext`, configurations, enums PostgreSQL |
| Repositorios | `ILegalizacionRepository`, `IEmpleadoRepository`, `ICatalogoRepository` |
| UnitOfWork | Transacciones |
| Tests dominio | Transiciones de estado, invariantes |

**Criterio de done:** `dotnet test` pasa; se puede leer/escribir legalización en BD.

---

## Fase 2 — Application layer + API catálogos

**Objetivo:** Casos de uso core y primeros endpoints.

| Tarea | Detalle |
|-------|---------|
| MediatR + FluentValidation | Pipeline behaviors |
| Commands | Crear/actualizar legalización, agregar gasto |
| Queries | Listar catálogos, obtener legalización, mis legalizaciones |
| DTOs | Request/response models |
| API | `CatalogosController`, `LegalizacionesController` (CRUD borrador) |
| Swagger | Documentación OpenAPI |

**Criterio de done:** Empleado puede crear legalización y agregar gastos vía API.

---

## Fase 3 — Autenticación

**Objetivo:** Login y autorización por rol.

| Tarea | Detalle |
|-------|---------|
| JWT Bearer | Generación y validación de tokens |
| `ICurrentUserService` | Empleado autenticado en contexto |
| `AuthController` | Login MVP (email + validación contra BD) |
| Autorización | Policies por rol (`Empleado`, `Jefe`, `Nomina`, `Admin`) |
| Middleware | Exception handling, ProblemDetails |

**Criterio de done:** Endpoints protegidos; cada rol ve solo lo permitido.

---

## Fase 4 — Documentos (MinIO + OCR)

**Objetivo:** Upload de soportes y extracción OCR.

| Tarea | Detalle |
|-------|---------|
| MinIO | `IFileStorageService`, upload/download |
| `SoportesController` | Multipart upload |
| Azure DI | `IOcrService`, procesamiento async |
| Commands | Subir soporte, procesar OCR, validar campos |
| Aplicar OCR | Copiar campos validados al gasto |

**Criterio de done:** Empleado sube factura → OCR extrae datos → valida → gasto se completa.

---

## Fase 5 — Workflow de aprobación

**Objetivo:** Flujo completo de estados.

| Tarea | Detalle |
|-------|---------|
| Commands workflow | Enviar, aprobar, rechazar, validar nómina, cerrar |
| `ILegalizacionWorkflowService` | Validar permisos jefe/empleado |
| Historial | Registrar comentarios en rechazo |
| Bandejas | Queries pendientes jefe y nómina |
| `BandejasController` | Endpoints de bandejas |

**Criterio de done:** Flujo completo BORRADOR → CERRADA funcional.

---

## Fase 6 — Pulido MVP

**Objetivo:** Production-ready básico.

| Tarea | Detalle |
|-------|---------|
| Health checks | `/health` (BD + MinIO) |
| Serilog | Logging estructurado |
| CORS | Configuración frontend |
| Docker Compose | API + PostgreSQL + MinIO local |
| README | Instrucciones de ejecución |

**Criterio de done:** Stack completo levanta con `docker compose up`.

---

## Diagrama de dependencias entre fases

```
Fase 0 ──► Fase 1 ──► Fase 2 ──► Fase 3
                      │
                      └──► Fase 4 ──► Fase 5 ──► Fase 6
```

Fase 3 (auth) puede paralelizarse parcialmente con Fase 4, pero Fase 5 requiere ambas.

---

## Stack de paquetes NuGet (por fase)

### Fase 1
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.EntityFrameworkCore.Design`

### Fase 2
- `MediatR`
- `FluentValidation`
- `FluentValidation.DependencyInjectionExtensions`

### Fase 3
- `Microsoft.AspNetCore.Authentication.JwtBearer`

### Fase 4
- `Minio`
- `Azure.AI.FormRecognizer`

### Fase 6
- `Serilog.AspNetCore`
- `AspNetCore.HealthChecks.NpgSql`
