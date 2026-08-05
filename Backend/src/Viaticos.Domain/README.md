# Viaticos.Domain

Núcleo del dominio. **Sin dependencias externas.**

## Módulos

| Carpeta | Contenido |
|---------|-----------|
| `Common/` | `Entity`, `AggregateRoot`, excepciones base |
| `Core/` | Empleado, Moneda, CategoriaGasto, Rol |
| `Legalizaciones/` | Legalizacion (agregado raíz), Gasto, historial, estados |
| `Documentos/` | Archivo, OcrExtraccion, OcrCampo |

## Reglas

- No referenciar Application, Infrastructure ni Api
- Lógica de negocio e invariantes viven aquí
- Persistencia ignorada (EF configs en Infrastructure)

Ver [ARCHITECTURE.md](../../../docs/ARCHITECTURE.md)
