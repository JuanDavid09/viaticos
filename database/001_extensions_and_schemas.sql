-- ============================================================
-- Viáticos MVP — Extensions and Schemas
-- Target: PostgreSQL 15+
-- ============================================================

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE SCHEMA IF NOT EXISTS core;
CREATE SCHEMA IF NOT EXISTS viaticos;
CREATE SCHEMA IF NOT EXISTS docs;

COMMENT ON SCHEMA core    IS 'Catálogos y usuarios';
COMMENT ON SCHEMA viaticos IS 'Legalizaciones, gastos y flujo de aprobación';
COMMENT ON SCHEMA docs    IS 'Archivos MinIO y OCR';
