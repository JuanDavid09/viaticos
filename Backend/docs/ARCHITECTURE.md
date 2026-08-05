# Arquitectura Backend — Viáticos MVP

> Ubicación: `Backend/` — solution .NET separado del frontend (`Frontend/`).

## Visión general

Backend **Modular Monolith** con **Clean Architecture** y **DDD táctico**, alineado al esquema PostgreSQL MVP (3 schemas, 10 tablas).

```
                    ┌─────────────────────────────────────┐
                    │           Viaticos.Api              │
                    │  Controllers · Middleware · Auth    │
                    └─────────────────┬───────────────────┘
                                      │ DTOs / Commands / Queries
                    ┌─────────────────▼───────────────────┐
                    │       Viaticos.Application          │
                    │  Use Cases · Validators · Ports     │
                    └─────────────────┬───────────────────┘
                                      │ Interfaces
          ┌───────────────────────────▼───────────────────────────┐
          │                    Viaticos.Domain                     │
          │   Aggregates · Entities · Value Objects · Domain Svc  │
          └───────────────────────────▲───────────────────────────┘
                                      │ Implementaciones
                    ┌─────────────────┴───────────────────┐
                    │      Viaticos.Infrastructure        │
                    │  EF Core · MinIO · Azure OCR · JWT  │
                    └─────────────────────────────────────┘
```

### Regla de dependencias

| Proyecto | Puede referenciar |
|----------|-------------------|
| `Domain` | *(nada — núcleo puro)* |
| `Application` | `Domain` |
| `Infrastructure` | `Application`, `Domain` |
| `Api` | `Application`, `Infrastructure` |

---

## Módulos (Bounded Contexts MVP)

Tres módulos reflejan los schemas de BD. Conviven en el mismo deploy pero con namespaces y carpetas separados.

| Módulo | Namespace | Schema BD | Responsabilidad |
|--------|-----------|-----------|-----------------|
| **Core** | `Viaticos.Domain.Core` | `core` | Empleados, catálogos (moneda, categoría) |
| **Legalizaciones** | `Viaticos.Domain.Legalizaciones` | `viaticos` | Legalizaciones, gastos, workflow |
| **Documentos** | `Viaticos.Domain.Documentos` | `docs` | Archivos MinIO, OCR |

---

## Modelo de dominio (DDD)

### Agregados

#### 1. `Legalizacion` (Agregado raíz — núcleo del negocio)

```
Legalizacion (Aggregate Root)
├── Gasto (Entity)
│   └── GastoSoporteId[] (referencias a Archivo)
└── LegalizacionHistorial[] (Entity — append-only)
```

**Invariantes:**
- `fecha_fin >= fecha_inicio`
- Solo editable en estados `BORRADOR` y `PENDIENTE_VALIDACION`
- Debe tener al menos 1 gasto para enviar a aprobación
- Cada gasto con categoría que requiere soporte debe tener adjunto
- Totales calculados por BD (trigger) — dominio delega persistencia

**Transiciones de estado (máquina de estados):**

```
                    enviarValidacion()
BORRADOR ──────────────────────────► PENDIENTE_VALIDACION
                                              │
                                    enviarAprobacion()
                                              ▼
                                    PENDIENTE_APROBACION
                                     │              │
                          aprobar()  │              │ rechazar()
                                     ▼              ▼
                                  APROBADA      RECHAZADA
                                     │              │
                              enviarNomina()       │ corregir()
                                     ▼              └──► BORRADOR
                              PENDIENTE_NOMINA
                                     │
                              validarNomina()
                                     ▼
                                  CERRADA
```

**Quién puede actuar:**

| Acción | Rol |
|--------|-----|
| Crear / editar gastos | EMPLEADO (propietario) |
| Enviar a aprobación | EMPLEADO (propietario) |
| Aprobar / Rechazar | JEFE_APROBADOR (jefe del empleado) |
| Validar / Cerrar | NOMINA |
| Ver todo | ADMIN |

#### 2. `Empleado` (Agregado — referenciado por ID)

Entidad de referencia. No se modifica desde el flujo de viáticos en MVP (solo lectura + auth).

#### 3. `Archivo` (Agregado — documentos)

```
Archivo (Aggregate Root)
└── OcrExtraccion (Entity)
    └── OcrCampo[] (Entity)
```

Proceso OCR asíncrono: subir → encolar OCR → completar → usuario valida campos → aplicar a Gasto.

### Value Objects (MVP)

| Value Object | Uso |
|--------------|-----|
| `Dinero` | Montos con validación > 0 |
| `PeriodoViaje` | fecha_inicio + fecha_fin |
| `NumeroLegalizacion` | Formato LEG-YYYY-NNNNN (generado en BD) |

### Domain Services

| Servicio | Responsabilidad |
|----------|-----------------|
| `ILegalizacionWorkflowService` | Validar permisos de transición según rol y relación jefe-empleado |
| `IOcrMappingService` | Mapear campos OCR → propiedades de Gasto |

---

## Capa Application (Casos de uso)

Patrón **CQRS ligero** con **MediatR**. Un handler por operación.

### Commands (escritura)

| Command | Módulo | Descripción |
|---------|--------|-------------|
| `CrearLegalizacionCommand` | Viaticos | Empleado crea borrador |
| `ActualizarLegalizacionCommand` | Viaticos | Editar motivo, fechas, anticipo |
| `AgregarGastoCommand` | Viaticos | Agregar línea de gasto |
| `ActualizarGastoCommand` | Viaticos | Modificar gasto |
| `EliminarGastoCommand` | Viaticos | Soft delete |
| `SubirSoporteCommand` | Documentos | Upload MinIO + vincular a gasto |
| `ProcesarOcrCommand` | Documentos | Invocar Azure DI |
| `ValidarCamposOcrCommand` | Documentos | Usuario corrige/confirma campos |
| `AplicarOcrAGastoCommand` | Viaticos | Copiar campos validados al gasto |
| `EnviarValidacionCommand` | Viaticos | BORRADOR → PENDIENTE_VALIDACION |
| `EnviarAprobacionCommand` | Viaticos | → PENDIENTE_APROBACION |
| `AprobarLegalizacionCommand` | Viaticos | Jefe aprueba |
| `RechazarLegalizacionCommand` | Viaticos | Jefe rechaza con comentario |
| `ValidarNominaCommand` | Viaticos | Nómina valida |
| `CerrarLegalizacionCommand` | Viaticos | Cierre final |

### Queries (lectura)

| Query | Descripción |
|-------|-------------|
| `ObtenerLegalizacionQuery` | Detalle con gastos y soportes |
| `ListarMisLegalizacionesQuery` | Bandeja del empleado |
| `ListarPendientesAprobacionQuery` | Bandeja del jefe |
| `ListarPendientesNominaQuery` | Bandeja nómina |
| `ListarCatalogosQuery` | Monedas + categorías |
| `ObtenerHistorialQuery` | Historial de estados |

### Ports (interfaces en Application)

```
Persistence/
├── ILegalizacionRepository
├── IEmpleadoRepository
├── ICatalogoRepository
└── IUnitOfWork

Storage/
└── IFileStorageService          → MinIO

Ocr/
└── IOcrService                  → Azure Document Intelligence

Identity/
└── ICurrentUserService          → Usuario autenticado actual
```

---

## Capa Infrastructure

| Componente | Tecnología |
|------------|------------|
| ORM | EF Core 8 + Npgsql |
| Storage | MinIO (.NET SDK) |
| OCR | Azure.AI.FormRecognizer |
| Auth MVP | JWT Bearer (login simple por email en dev) |
| Validación | FluentValidation |
| Mapping | Manual / Mapster (evaluar en Fase 2) |
| Logging | Serilog |

### DbContext

Un solo `ViaticosDbContext` con schemas mapeados:

```csharp
modelBuilder.HasDefaultSchema("public");
// Entidades mapeadas a core.*, viaticos.*, docs.*
```

Convención: configuraciones EF en `Infrastructure/Persistence/Configurations/`.

---

## Capa API

### Controllers MVP

| Controller | Endpoints base |
|------------|----------------|
| `AuthController` | `POST /api/auth/login` (dev) |
| `CatalogosController` | `GET /api/catalogos/monedas`, `/categorias` |
| `LegalizacionesController` | CRUD + workflow |
| `GastosController` | CRUD anidado en legalización |
| `SoportesController` | Upload + OCR |
| `BandejasController` | Mis legalizaciones, pendientes jefe/nómina |

### Middleware

- `ExceptionHandlingMiddleware` — respuestas ProblemDetails
- `CorrelationIdMiddleware` — trazabilidad
- Autenticación JWT + autorización por rol (claims)

---

## Estructura de carpetas

```
Backend/
├── src/
│   ├── Viaticos.Domain/
│   ├── Common/
│   ├── Core/
│   │   ├── Entities/
│   │   └── Enums/
│   ├── Legalizaciones/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Services/
│   └── Documentos/
│       ├── Entities/
│       └── Enums/
│
├── Viaticos.Application/
│   ├── Common/
│   │   ├── Behaviors/          # Validation, Logging
│   │   ├── Interfaces/
│   │   └── Models/             # Result, PaginatedList
│   ├── Core/
│   ├── Legalizaciones/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   └── DTOs/
│   └── Documentos/
│
├── Viaticos.Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   ├── Repositories/
│   │   └── ViaticosDbContext.cs
│   ├── Storage/
│   ├── Ocr/
│   ├── Identity/
│   └── DependencyInjection.cs
│
└── Viaticos.Api/
    ├── Controllers/
    ├── Middleware/
    ├── Extensions/
    └── appsettings.json
```

---

## Decisiones de diseño MVP

| Decisión | Elección | Razón |
|----------|----------|-------|
| Monolito vs microservicios | Modular Monolith | MVP simple, un deploy |
| CQRS | Ligero (MediatR) | Separación clara sin over-engineering |
| Eventos de dominio | Diferidos | Historial en BD cubre trazabilidad |
| Totales | Trigger PostgreSQL | Ya implementado, evita duplicar lógica |
| Auth | JWT simple | SSO (Azure AD) en fase futura vía `auth_subject_id` |
| Soft delete | `deleted_at` en BD | Queries filtran en repositorio |
| Validación | FluentValidation + dominio | Doble capa: input + invariantes |

---

## Integraciones externas

```
┌──────────┐     ┌──────────┐     ┌─────────────────────┐
│ Frontend │────►│   API    │────►│ PostgreSQL          │
└──────────┘     │          │     └─────────────────────┘
                 │          │────►│ MinIO (soportes)    │
                 │          │     └─────────────────────┘
                 │          │────►│ Azure DI (OCR)      │
                 └──────────┘     └─────────────────────┘
```

---

## Referencias

- [PHASES.md](./PHASES.md) — Plan de implementación por fases
- [Base de datos](../../database/README.md) — Esquema de BD
