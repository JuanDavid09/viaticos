# Viáticos Platform

Plataforma interna de gestión y legalización de viáticos.

## Stack

| Capa | Tecnología |
|------|------------|
| Frontend | React + TypeScript *(pendiente — carpeta `Frontend/`)* |
| Backend | ASP.NET Core 8 — Clean Architecture + DDD |
| Base de datos | PostgreSQL 15+ |
| Storage | MinIO *(Fase 4)* |
| OCR | Azure Document Intelligence *(Fase 4)* |

## Estructura del repositorio

```
Viaticos/
├── Backend/           # API .NET Core (solution, src, tests, docs)
├── Frontend/          # React + TypeScript (pendiente)
├── database/          # Scripts SQL MVP (compartido)
└── README.md
```

## Documentación

- [Backend — Arquitectura](Backend/docs/ARCHITECTURE.md)
- [Backend — Plan de fases](Backend/docs/PHASES.md)
- [Base de datos](database/README.md)

## Desarrollo

```powershell
# Base de datos
cd database
psql -U postgres -d viaticos -f run_all.sql

# Backend
cd Backend
dotnet build
dotnet test
dotnet run --project src/Viaticos.Api
```

## Fase actual

**Fase 0** — Fundamentos: solution, arquitectura, entidades de dominio base.

Siguiente: **Fase 1** — EF Core + persistencia PostgreSQL.
