-- ============================================================
-- Viáticos MVP — DROP ALL (solo desarrollo)
-- ============================================================

DROP VIEW IF EXISTS viaticos.v_gasto_activo CASCADE;
DROP VIEW IF EXISTS viaticos.v_legalizacion_activa CASCADE;

DROP SCHEMA IF EXISTS viaticos CASCADE;
DROP SCHEMA IF EXISTS docs CASCADE;
DROP SCHEMA IF EXISTS core CASCADE;

DROP TYPE IF EXISTS docs.estado_ocr_enum CASCADE;
DROP TYPE IF EXISTS viaticos.estado_legalizacion_enum CASCADE;
DROP TYPE IF EXISTS core.rol_enum CASCADE;
