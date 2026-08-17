-- ============================================================
-- 010: Notificaciones in-app
-- ============================================================

CREATE TABLE IF NOT EXISTS core.notificacion (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    destinatario_id UUID NOT NULL REFERENCES core.empleado(id),
    tipo            VARCHAR(50) NOT NULL,
    titulo          VARCHAR(200) NOT NULL,
    mensaje         TEXT NOT NULL,
    entidad_tipo    VARCHAR(50),
    entidad_id      UUID,
    leida           BOOLEAN NOT NULL DEFAULT FALSE,
    leida_at        TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_notificacion_destinatario_leida
    ON core.notificacion (destinatario_id, leida, created_at DESC);
