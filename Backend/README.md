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

**Fase 2** completada — casos de uso y API MVP.

Siguiente: **Fase 3** — Autenticación JWT.

### Endpoints MVP

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/health` | Estado BD |
| GET | `/api/catalogos` | Monedas + categorías |
| GET | `/api/legalizaciones` | Mis legalizaciones |
| GET | `/api/legalizaciones/{id}` | Detalle |
| POST | `/api/legalizaciones` | Crear borrador |
| PUT | `/api/legalizaciones/{id}` | Actualizar borrador |
| POST | `/api/legalizaciones/{id}/gastos` | Agregar gasto |

### Usuario de desarrollo (MVP)

Header opcional en todos los endpoints (excepto health):

```http
X-Dev-User-Email: empleado@empresa.com
```

Usuarios seed: `empleado@empresa.com`, `jefe@empresa.com`, `nomina@empresa.com`, `admin@empresa.com`

### Probar con Swagger

```powershell
dotnet run --project src/Viaticos.Api
# Abrir /swagger
```
