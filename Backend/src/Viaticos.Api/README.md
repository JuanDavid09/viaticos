# Viaticos.Api

Capa de presentación HTTP. Controllers delgados que delegan a MediatR.

## Estructura

```
Controllers/     → REST endpoints
Middleware/      → Exception handling, correlation ID
Extensions/      → DI setup, Swagger
```

## Reglas

- Sin lógica de negocio en controllers
- Auth y autorización por rol (Fase 3)
