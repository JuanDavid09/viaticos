-- ============================================================
-- Viáticos MVP — Custom Types
-- ============================================================

CREATE TYPE core.rol_enum AS ENUM (
    'EMPLEADO',
    'JEFE_APROBADOR',
    'NOMINA',
    'ADMIN'
);

CREATE TYPE viaticos.estado_legalizacion_enum AS ENUM (
    'BORRADOR',
    'PENDIENTE_VALIDACION',
    'PENDIENTE_APROBACION',
    'APROBADA',
    'RECHAZADA',
    'PENDIENTE_NOMINA',
    'CERRADA'
);

CREATE TYPE docs.estado_ocr_enum AS ENUM (
    'PENDIENTE',
    'PROCESANDO',
    'COMPLETADO',
    'ERROR',
    'VALIDADO_USUARIO'
);
