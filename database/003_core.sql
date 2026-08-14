-- ============================================================
-- Viáticos MVP — Schema core
-- ============================================================

CREATE TABLE core.moneda (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    codigo_iso  CHAR(3) NOT NULL,
    nombre      VARCHAR(50) NOT NULL,
    simbolo     VARCHAR(5),
    activo      BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT uq_moneda_codigo UNIQUE (codigo_iso)
);

CREATE TABLE core.categoria_gasto (
    id               UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    codigo           VARCHAR(30) NOT NULL,
    nombre           VARCHAR(100) NOT NULL,
    requiere_soporte BOOLEAN NOT NULL DEFAULT TRUE,
    activo           BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT uq_categoria_codigo UNIQUE (codigo)
);

CREATE TABLE core.empleado (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    codigo_empleado     VARCHAR(30) NOT NULL,
    email               VARCHAR(254) NOT NULL,
    nombre              VARCHAR(100) NOT NULL,
    apellido            VARCHAR(100) NOT NULL,
    departamento        VARCHAR(100),
    rol                 core.rol_enum NOT NULL DEFAULT 'EMPLEADO',
    jefe_id             UUID REFERENCES core.empleado(id),
    auth_subject_id     VARCHAR(255),
    password_hash       VARCHAR(255),
    must_change_password BOOLEAN NOT NULL DEFAULT FALSE,
    activo              BOOLEAN NOT NULL DEFAULT TRUE,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_empleado_codigo UNIQUE (codigo_empleado),
    CONSTRAINT uq_empleado_email UNIQUE (email)
);

COMMENT ON TABLE core.moneda IS 'Monedas soportadas (COP, USD, etc.)';
COMMENT ON TABLE core.categoria_gasto IS 'Tipos de gasto viáticos';
COMMENT ON TABLE core.empleado IS 'Usuarios del sistema con rol y jefe directo';
COMMENT ON COLUMN core.empleado.jefe_id IS 'Jefe aprobador para el flujo de viáticos';
COMMENT ON COLUMN core.empleado.auth_subject_id IS 'ID externo SSO (Azure AD / OIDC) — futuro';
COMMENT ON COLUMN core.empleado.password_hash IS 'Hash de contraseña (ASP.NET Identity PasswordHasher v3)';
COMMENT ON COLUMN core.empleado.must_change_password IS 'Obliga cambio de contraseña en el próximo acceso';
