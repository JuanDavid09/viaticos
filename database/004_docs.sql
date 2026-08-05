-- ============================================================
-- Viáticos MVP — Schema docs (MinIO + OCR)
-- ============================================================

CREATE TABLE docs.archivo (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    bucket          VARCHAR(100) NOT NULL,
    object_key      VARCHAR(500) NOT NULL,
    nombre_original VARCHAR(255) NOT NULL,
    mime_type       VARCHAR(100) NOT NULL,
    tamano_bytes    BIGINT NOT NULL,
    subido_por      UUID NOT NULL REFERENCES core.empleado(id),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_archivo_object UNIQUE (bucket, object_key),
    CONSTRAINT chk_archivo_tamano CHECK (tamano_bytes > 0)
);

CREATE TABLE docs.ocr_extraccion (
    id                 UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    archivo_id         UUID NOT NULL REFERENCES docs.archivo(id),
    gasto_soporte_id   UUID,
    azure_operation_id VARCHAR(100),
    estado             docs.estado_ocr_enum NOT NULL DEFAULT 'PENDIENTE',
    json_respuesta     JSONB,
    error_mensaje      TEXT,
    procesado_at       TIMESTAMPTZ,
    created_at         TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE docs.ocr_campo (
    id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ocr_extraccion_id UUID NOT NULL REFERENCES docs.ocr_extraccion(id) ON DELETE CASCADE,
    nombre_campo      VARCHAR(100) NOT NULL,
    valor_extraido    TEXT,
    valor_validado    TEXT,
    validado          BOOLEAN NOT NULL DEFAULT FALSE,
    validado_por      UUID REFERENCES core.empleado(id),
    validado_at       TIMESTAMPTZ
);

COMMENT ON TABLE docs.archivo IS 'Metadatos de archivos en MinIO';
COMMENT ON TABLE docs.ocr_extraccion IS 'Resultado OCR de Azure Document Intelligence';
COMMENT ON TABLE docs.ocr_campo IS 'Campos extraídos y corregidos por el usuario';
