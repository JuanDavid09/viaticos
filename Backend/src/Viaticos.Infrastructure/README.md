# Viaticos.Infrastructure

Implementaciones técnicas: EF Core, MinIO, Azure OCR, JWT.

## Estructura

```
Persistence/     → DbContext, configurations, repositories
Storage/         → MinIO file storage
Ocr/             → Azure Document Intelligence
Identity/        → CurrentUserService, JWT
DependencyInjection.cs
```

## Reglas

- Implementa interfaces de `Application`
- Único proyecto con referencias a paquetes de infraestructura
