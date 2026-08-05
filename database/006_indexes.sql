-- ============================================================
-- Viáticos MVP — Indexes
-- ============================================================

-- core
CREATE INDEX idx_empleado_rol
    ON core.empleado (rol)
    WHERE activo = TRUE;

CREATE INDEX idx_empleado_jefe
    ON core.empleado (jefe_id)
    WHERE activo = TRUE;

CREATE INDEX idx_empleado_email
    ON core.empleado (email);

-- docs
CREATE INDEX idx_archivo_subido_por
    ON docs.archivo (subido_por, created_at DESC);

CREATE INDEX idx_ocr_pendientes
    ON docs.ocr_extraccion (estado, created_at)
    WHERE estado IN ('PENDIENTE', 'PROCESANDO');

CREATE INDEX idx_ocr_archivo
    ON docs.ocr_extraccion (archivo_id);

-- viaticos
CREATE INDEX idx_legalizacion_empleado
    ON viaticos.legalizacion (empleado_id, estado)
    WHERE deleted_at IS NULL;

CREATE INDEX idx_legalizacion_estado
    ON viaticos.legalizacion (estado, submitted_at DESC NULLS LAST)
    WHERE deleted_at IS NULL;

CREATE INDEX idx_legalizacion_numero
    ON viaticos.legalizacion (numero);

CREATE INDEX idx_gasto_legalizacion
    ON viaticos.gasto (legalizacion_id, orden)
    WHERE deleted_at IS NULL;

CREATE INDEX idx_historial_legalizacion
    ON viaticos.legalizacion_historial (legalizacion_id, created_at DESC);
