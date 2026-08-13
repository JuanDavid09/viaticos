# Backend — Viáticos

API ASP.NET Core 8 con Clean Architecture y DDD.

## Estructura

```
Backend/
├── Viaticos.sln
├── Directory.Build.props
├── docs/
│   ├── ARCHITECTURE.md
│   └── PHASES.md
├── src/
│   ├── Viaticos.Domain/
│   ├── Viaticos.Application/
│   ├── Viaticos.Infrastructure/
│   └── Viaticos.Api/
└── tests/
    └── Viaticos.Domain.Tests/
```

## Requisitos

- .NET 8 SDK
- PostgreSQL 15+ (ver [`../database/`](../database/))

## Comandos

```powershell
# Desde esta carpeta (Backend/)
dotnet build
dotnet test
dotnet run --project src/Viaticos.Api
```

## Abrir en Visual Studio / Rider

Abrir `Viaticos.sln` en esta carpeta.

## Documentación

- [Arquitectura](docs/ARCHITECTURE.md)
- [Plan de fases](docs/PHASES.md)

## Fase actual

**MVP Backend completado** (Fases 0–6).

## Docker (stack completo)

Desde la raíz del repositorio:

```powershell
docker compose up --build
```

- API: http://localhost:8080/swagger
- Health: http://localhost:8080/health
- MinIO: http://localhost:9001

## Desarrollo local

```powershell
dotnet build
dotnet test
dotnet run --project src/Viaticos.Api
```

## Observabilidad y configuración

| Feature | Detalle |
|---------|---------|
| `/health` | PostgreSQL + MinIO (o almacenamiento local) |
| `/api/health` | Check rápido de BD (legacy) |
| Serilog | Logs estructurados en consola |
| CORS | `Cors:AllowedOrigins` en appsettings |
| HTTPS redirect | `Api:UseHttpsRedirection` (false en Docker) |

### Endpoints MVP

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | `/health` | Anónimo | Health check (BD + storage) |
| GET | `/api/health` | Anónimo | Check rápido BD |
| POST | `/api/auth/login` | Anónimo | Login MVP (email) |
| GET | `/api/catalogos` | JWT | Monedas + categorías |
| GET | `/api/legalizaciones` | JWT | Mis legalizaciones |
| GET | `/api/bandejas/mis-legalizaciones` | JWT | Bandeja empleado |
| GET | `/api/bandejas/pendientes-aprobacion` | Jefe | Bandeja jefe |
| GET | `/api/bandejas/pendientes-nomina` | Nómina | Bandeja nómina |
| GET | `/api/legalizaciones/{id}` | JWT | Detalle (incluye soportes) |
| GET | `/api/legalizaciones/{id}/historial` | JWT | Historial de estados |
| POST | `/api/legalizaciones` | JWT | Crear borrador |
| PUT | `/api/legalizaciones/{id}` | JWT | Actualizar borrador |
| POST | `/api/legalizaciones/{id}/gastos` | JWT | Agregar gasto |
| POST | `/api/legalizaciones/{id}/enviar-validacion` | JWT | BORRADOR → PENDIENTE_VALIDACION |
| POST | `/api/legalizaciones/{id}/enviar-aprobacion` | JWT | → PENDIENTE_APROBACION |
| POST | `/api/legalizaciones/{id}/aprobar` | Jefe | → APROBADA |
| POST | `/api/legalizaciones/{id}/rechazar` | Jefe | → RECHAZADA |
| POST | `/api/legalizaciones/{id}/reabrir` | JWT | RECHAZADA → BORRADOR |
| POST | `/api/legalizaciones/{id}/enviar-nomina` | JWT | APROBADA → PENDIENTE_NOMINA |
| POST | `/api/legalizaciones/{id}/cerrar` | Nómina | → CERRADA |
| POST | `/api/soportes` | JWT | Subir soporte (multipart) |
| POST | `/api/soportes/{id}/ocr/procesar` | JWT | Ejecutar OCR |
| PUT | `/api/soportes/{id}/ocr/campos` | JWT | Validar campos OCR |
| POST | `/api/soportes/{id}/ocr/aplicar` | JWT | Aplicar OCR al gasto |

### Autenticación (MVP)

1. Obtener token:

```http
POST /api/auth/login
Content-Type: application/json

{ "email": "empleado@empresa.com" }
```

2. Usar en requests protegidos:

```http
Authorization: Bearer {accessToken}
```

Usuarios seed: `empleado@empresa.com`, `jefe@empresa.com`, `nomina@empresa.com`, `admin@empresa.com`

Configuración JWT en `src/Viaticos.Api/appsettings.json` (`Jwt:Secret`, `Issuer`, `Audience`, `ExpirationMinutes`).

### Documentos (Fase 4)

Por defecto en desarrollo:
- **Almacenamiento:** carpeta local `uploads/` (`Minio:UseLocalFallback: true`)
- **OCR:** servicio simulado (`MockOcrService`) si `AzureOcr:Endpoint` está vacío

Para MinIO real, configure `Minio:UseLocalFallback: false` y levante MinIO en `localhost:9000`.

Flujo OCR:
1. `POST /api/soportes` — subir factura/recibo (JPG, PNG, PDF, máx. 10 MB)
2. `POST /api/soportes/{id}/ocr/procesar` — extraer campos
3. `PUT /api/soportes/{id}/ocr/campos` — corregir/validar campos
4. `POST /api/soportes/{id}/ocr/aplicar` — copiar al gasto

### Workflow (Fase 5)

Usuarios seed: `empleado@empresa.com`, `jefe@empresa.com`, `nomina@empresa.com`

```
BORRADOR → enviar-validacion → PENDIENTE_VALIDACION
         → enviar-aprobacion → PENDIENTE_APROBACION
         → aprobar (jefe)    → APROBADA
         → rechazar (jefe)   → RECHAZADA → reabrir → BORRADOR
         → enviar-nomina     → PENDIENTE_NOMINA
         → cerrar (nómina)   → CERRADA
```

### Probar con Swagger

```powershell
dotnet run --project src/Viaticos.Api
# Abrir /swagger → Authorize con Bearer token
```
