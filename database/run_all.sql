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

\echo ''
\echo '=== MVP instalado exitosamente ==='
\echo 'Schemas: core, viaticos, docs'
\echo 'Tablas:  10'
\echo ''
