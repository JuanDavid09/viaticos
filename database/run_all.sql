-- ============================================================
-- Viáticos MVP — Master Script
--
-- Uso:
--   psql -U postgres -d viaticos -f database/run_all.sql
-- ============================================================

\set ON_ERROR_STOP on
\timing on

\echo '>>> 001: Extensions and Schemas'
\i 001_extensions_and_schemas.sql

\echo '>>> 002: Types'
\i 002_types.sql

\echo '>>> 003: Core'
\i 003_core.sql

\echo '>>> 004: Docs'
\i 004_docs.sql

\echo '>>> 005: Viaticos'
\i 005_viaticos.sql

\echo '>>> 006: Indexes'
\i 006_indexes.sql

\echo '>>> 007: Functions, Triggers and Views'
\i 007_functions_triggers.sql

\echo '>>> 008: Seed Data'
\i 008_seed_data.sql

\echo '>>> 010: Notificaciones'
\i 010_notificaciones.sql

\echo '>>> 011: Reportes (procedimientos almacenados)'
\i 011_reportes.sql

\echo '>>> 012: Backfill jefe_id empleados'
\i 012_empleado_jefe_backfill.sql

\echo ''
\echo '=== MVP instalado exitosamente ==='
\echo 'Schemas: core, viaticos, docs, reportes'
\echo 'Tablas:  11'
\echo 'Reportes: 13 procedimientos en schema reportes'
\echo ''
