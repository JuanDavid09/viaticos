# Viáticos Platform

Plataforma interna de gestión y legalización de viáticos — **MVP Backend completado**.

## Stack

| Capa | Tecnología |
|------|------------|
| Frontend | React + TypeScript *(pendiente — carpeta `Frontend/`)* |
| Backend | ASP.NET Core 8 — Clean Architecture + DDD |
| Base de datos | PostgreSQL 15+ |
| Storage | MinIO (Docker) / carpeta local (dev) |
| OCR | Mock local / Azure Document Intelligence |

## Estructura del repositorio

```
Viaticos/
├── Backend/           # API .NET Core (solution, src, tests, docs)
├── Frontend/          # React + TypeScript (pendiente)
├── database/          # Scripts SQL MVP (compartido)
├── docker-compose.yml # Stack MVP: API + PostgreSQL + MinIO
└── README.md
```

## Inicio rápido con Docker (recomendado)

Requisitos: [Docker Desktop](https://www.docker.com/products/docker-desktop/)

```powershell
# Desde la raíz del repo
docker compose up --build
```

| Servicio | URL |
|----------|-----|
| API + Swagger | http://localhost:8080/swagger |
| Health check | http://localhost:8080/health |
| MinIO Console | http://localhost:9001 (minioadmin / minioadmin) |
| PostgreSQL | localhost:5432 (postgres / postgres) |

La base de datos se inicializa automáticamente con los scripts de `database/`.

### Usuarios demo

| Email | Rol |
|-------|-----|
| empleado@empresa.com | EMPLEADO |
| jefe@empresa.com | JEFE_APROBADOR |
| nomina@empresa.com | NOMINA |
| admin@empresa.com | ADMIN |

Login: `POST http://localhost:8080/api/auth/login` con `{ "email": "empleado@empresa.com" }`

## Desarrollo local (sin Docker)

```powershell
# 1. Base de datos
psql -U postgres -d viaticos -f database/run_all.sql

# 2. Backend
cd Backend
dotnet build
dotnet test
dotnet run --project src/Viaticos.Api
```

Ajuste `ConnectionStrings:DefaultConnection` en `Backend/src/Viaticos.Api/appsettings.json`.

## Documentación

- [Backend — Arquitectura](Backend/docs/ARCHITECTURE.md)
- [Backend — Plan de fases](Backend/docs/PHASES.md)
- [Base de datos](database/README.md)
- [Backend — README](Backend/README.md)

## Estado del MVP

**Backend MVP completado** (Fases 0–6): auth JWT, legalizaciones, workflow, soportes/OCR, Docker.

Siguiente: **Frontend** React + TypeScript.
