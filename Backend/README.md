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

**Fase 3** completada — autenticación JWT y autorización por rol.

Siguiente: **Fase 4** — Documentos (MinIO + OCR).

### Endpoints MVP

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | `/api/health` | Anónimo | Estado BD |
| POST | `/api/auth/login` | Anónimo | Login MVP (email) |
| GET | `/api/catalogos` | JWT | Monedas + categorías |
| GET | `/api/legalizaciones` | JWT | Mis legalizaciones |
| GET | `/api/legalizaciones/{id}` | JWT | Detalle |
| POST | `/api/legalizaciones` | JWT | Crear borrador |
| PUT | `/api/legalizaciones/{id}` | JWT | Actualizar borrador |
| POST | `/api/legalizaciones/{id}/gastos` | JWT | Agregar gasto |

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

### Probar con Swagger

```powershell
dotnet run --project src/Viaticos.Api
# Abrir /swagger → Authorize con Bearer token
```
