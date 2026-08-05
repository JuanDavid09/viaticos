# Viaticos.Application

Casos de uso (CQRS con MediatR). Orquesta dominio sin lógica de infraestructura.

## Estructura

```
Common/          → Behaviors, interfaces (ports), Result
Core/            → Queries de catálogos
Viaticos/        → Commands/Queries de legalizaciones
Documentos/        → Commands/Queries de archivos y OCR
```

## Reglas

- Depende solo de `Domain`
- Define interfaces que `Infrastructure` implementa
- Validación de input con FluentValidation (Fase 2+)
