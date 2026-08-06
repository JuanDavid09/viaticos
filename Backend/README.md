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

**Fase 1** completada — EF Core, repositorios, conexión PostgreSQL.

Siguiente: **Fase 2** — Application layer + API catálogos.

### Configuración BD

Editar `src/Viaticos.Api/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=viaticos;Username=postgres;Password=TU_PASSWORD"
}
```

### Verificar conexión

```powershell
dotnet run --project src/Viaticos.Api
# GET https://localhost:7xxx/api/health
```

### Test de integración con PostgreSQL

```powershell
$env:VIATICOS_INTEGRATION_TESTS = "1"
dotnet test tests/Viaticos.Infrastructure.Tests
```
