-- ============================================================
-- Viáticos — Ejemplos de uso de reportes
-- Ejecutar después de 011_reportes.sql
-- ============================================================

\echo '=== Resumen por estado ==='
SELECT * FROM reportes.sp_resumen_por_estado();

\echo '=== Legalizaciones detalle (último trimestre) ==='
SELECT *
FROM reportes.sp_legalizaciones_detalle(
    p_fecha_desde := DATE_TRUNC('quarter', CURRENT_DATE)::DATE,
    p_fecha_hasta := CURRENT_DATE
)
LIMIT 20;

\echo '=== Gastos por categoría ==='
SELECT * FROM reportes.sp_gastos_por_categoria();

\echo '=== Pendientes de aprobación ==='
SELECT * FROM reportes.sp_pendientes_aprobacion();

\echo '=== Pendientes de nómina ==='
SELECT * FROM reportes.sp_pendientes_nomina();

\echo '=== Calendario del mes actual ==='
SELECT *
FROM reportes.sp_calendario_viaticos(
    p_desde := DATE_TRUNC('month', CURRENT_DATE)::DATE,
    p_hasta := (DATE_TRUNC('month', CURRENT_DATE) + INTERVAL '1 month - 1 day')::DATE
);

\echo '=== Gastos sin soporte ==='
SELECT * FROM reportes.sp_gastos_sin_soporte();

\echo '=== Volumen mensual ==='
SELECT * FROM reportes.sp_volumen_mensual(p_anio := EXTRACT(YEAR FROM CURRENT_DATE)::INTEGER);
