# Database — Viáticos MVP

Esquema PostgreSQL simplificado para el flujo principal de legalización de viáticos.

## Alcance MVP

| Incluido | Descripción |
|----------|-------------|
| 3 schemas | `core`, `viaticos`, `docs` |
| 10 tablas | Solo lo necesario para el flujo |
| 4 roles | Enum en `empleado.rol` (sin tablas de permisos) |
| OCR | Archivos MinIO + extracción Azure DI |
| Workflow | 7 estados + historial de cambios |
| Totales | Recalculados automáticamente por trigger |

| Diferido (futuro) | Motivo |
|-------------------|--------|
| Schema `erp` | Integración ERP no requerida aún |
| Schema `auth` | Roles manejados por enum + ASP.NET Identity |
| Schema `audit` | Historial de estados cubre trazabilidad básica |
| Políticas de topes | Reglas de negocio en aplicación |
| Multi-empresa | Single tenant por ahora |

## Estructura de scripts

| Script | Contenido |
|--------|-----------|
| `001_extensions_and_schemas.sql` | Extensiones y schemas |
| `002_types.sql` | ENUMs (rol, estado, OCR) |
| `003_core.sql` | Moneda, categorías, empleados |
| `004_docs.sql` | Archivos y OCR |
| `005_viaticos.sql` | Legalizaciones, gastos, historial |
| `006_indexes.sql` | Índices esenciales |
| `007_functions_triggers.sql` | Numeración, totales, historial |
| `008_seed_data.sql` | Datos demo |
| `010_notificaciones.sql` | Notificaciones in-app |
| `011_reportes.sql` | Procedimientos almacenados para reportes |
| `011_reportes_ejemplos.sql` | Consultas de ejemplo para reportes |
| `run_all.sql` | Script maestro |
| `000_drop_all.sql` | Reset (solo desarrollo) |

## Modelo de datos

```
core.empleado ──< viaticos.legalizacion ──< viaticos.gasto ──< viaticos.gasto_soporte
                      │                         │                      │
                      │                         │                      └── docs.archivo
                      │                         │                              └── docs.ocr_extraccion
                      │                         │                                      └── docs.ocr_campo
                      └── viaticos.legalizacion_historial

core.moneda ──< viaticos.legalizacion
core.categoria_gasto ──< viaticos.gasto
```

## Flujo de estados

```
BORRADOR → PENDIENTE_VALIDACION → PENDIENTE_APROBACION → APROBADA → PENDIENTE_NOMINA → CERRADA
                                              ↓
                                          RECHAZADA
```

## Instalación

```powershell
psql -U postgres -c "CREATE DATABASE viaticos ENCODING 'UTF8';"
cd d:\Projects\Viaticos\database
psql -U postgres -d viaticos -f run_all.sql
```

## Usuarios demo (seed)

| Código | Email | Rol | Contraseña demo |
|--------|-------|-----|-----------------|
| ADM001 | admin@empresa.com | ADMIN | Admin123! |
| NOM001 | nomina@empresa.com | NOMINA | Cambiar123! |
| JEF001 | jefe@empresa.com | JEFE_APROBADOR | Cambiar123! |
| EMP001 | empleado@empresa.com | EMPLEADO (jefe: JEF001) | Cambiar123! |
| EMP002 | empleado2@empresa.com | EMPLEADO (jefe: JEF001) | Cambiar123! |

Usuarios distintos de admin deben cambiar la contraseña en el primer ingreso (`must_change_password = true`).

### Upgrade en BD existente

```powershell
psql -U postgres -d viaticos -f 009_auth_password.sql
psql -U postgres -d viaticos -f 010_notificaciones.sql
psql -U postgres -d viaticos -f 011_reportes.sql
```

## Reportes (procedimientos almacenados)

Los reportes viven en el schema `reportes` como funciones `sp_*` que retornan tablas.

| Procedimiento | Uso |
|---------------|-----|
| `sp_resumen_por_estado` | Totales por estado (dashboard gerencial) |
| `sp_legalizaciones_detalle` | Listado exportable de legalizaciones |
| `sp_gastos_por_categoria` | Consolidado de gastos por categoría |
| `sp_gastos_detalle` | Detalle línea a línea para nómina/ERP |
| `sp_resumen_financiero_empleado` | Reembolsos y devoluciones por empleado |
| `sp_pendientes_aprobacion` | Bandeja del jefe aprobador |
| `sp_pendientes_nomina` | Bandeja de cierre de nómina |
| `sp_legalizaciones_cerradas` | Cierres en un periodo |
| `sp_calendario_viaticos` | Viajes por rango de fechas |
| `sp_gastos_sin_soporte` | Cumplimiento documental |
| `sp_historial_auditoria` | Trazabilidad de cambios de estado |
| `sp_volumen_mensual` | Tendencia mensual de volumen y montos |
| `sp_tiempos_por_estado` | SLA / horas en cada estado |

Ejemplo:

```sql
SELECT * FROM reportes.sp_pendientes_aprobacion();
SELECT * FROM reportes.sp_resumen_financiero_empleado(
    p_fecha_desde := '2026-01-01',
    p_fecha_hasta := '2026-12-31'
);
```

Ver más ejemplos en `011_reportes_ejemplos.sql`.

## Reset (desarrollo)

```powershell
psql -U postgres -d viaticos -f 000_drop_all.sql
psql -U postgres -d viaticos -f run_all.sql
```
