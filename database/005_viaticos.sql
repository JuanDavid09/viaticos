-- ============================================================
-- Viáticos MVP — Schema viaticos (núcleo del negocio)
-- ============================================================

CREATE SEQUENCE viaticos.legalizacion_numero_seq START WITH 1 INCREMENT BY 1;

CREATE TABLE viaticos.legalizacion (
    id               UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    numero           VARCHAR(30) NOT NULL,
    empleado_id      UUID NOT NULL REFERENCES core.empleado(id),
    motivo           TEXT NOT NULL,
    destino          VARCHAR(200),
    fecha_inicio     DATE NOT NULL,
    fecha_fin        DATE NOT NULL,
    moneda_id        UUID NOT NULL REFERENCES core.moneda(id),
    monto_anticipo   NUMERIC(18,2) NOT NULL DEFAULT 0,
    estado           viaticos.estado_legalizacion_enum NOT NULL DEFAULT 'BORRADOR',
    total_gastos     NUMERIC(18,2) NOT NULL DEFAULT 0,
    total_reembolso  NUMERIC(18,2) NOT NULL DEFAULT 0,
    total_devolucion NUMERIC(18,2) NOT NULL DEFAULT 0,
    observaciones    TEXT,
    deleted_at       TIMESTAMPTZ,
    created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by       UUID NOT NULL REFERENCES core.empleado(id),
    updated_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by       UUID REFERENCES core.empleado(id),
    submitted_at     TIMESTAMPTZ,
    closed_at        TIMESTAMPTZ,
    CONSTRAINT uq_legalizacion_numero UNIQUE (numero),
    CONSTRAINT chk_legalizacion_fechas CHECK (fecha_fin >= fecha_inicio),
    CONSTRAINT chk_legalizacion_anticipo CHECK (monto_anticipo >= 0)
);

CREATE TABLE viaticos.gasto (
    id                 UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    legalizacion_id    UUID NOT NULL REFERENCES viaticos.legalizacion(id),
    categoria_gasto_id UUID NOT NULL REFERENCES core.categoria_gasto(id),
    fecha_gasto        DATE NOT NULL,
    descripcion        TEXT NOT NULL,
    proveedor          VARCHAR(200),
    numero_documento   VARCHAR(50),
    monto              NUMERIC(18,2) NOT NULL,
    validado           BOOLEAN NOT NULL DEFAULT FALSE,
    validado_por       UUID REFERENCES core.empleado(id),
    validado_at        TIMESTAMPTZ,
    orden              SMALLINT NOT NULL DEFAULT 0,
    deleted_at         TIMESTAMPTZ,
    created_at         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by         UUID NOT NULL REFERENCES core.empleado(id),
    updated_at         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_gasto_monto CHECK (monto > 0)
);

CREATE TABLE viaticos.gasto_soporte (
    id           UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    gasto_id     UUID NOT NULL REFERENCES viaticos.gasto(id) ON DELETE CASCADE,
    archivo_id   UUID NOT NULL REFERENCES docs.archivo(id),
    es_principal BOOLEAN NOT NULL DEFAULT FALSE,
    created_at   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by   UUID NOT NULL REFERENCES core.empleado(id),
    CONSTRAINT uq_gasto_archivo UNIQUE (gasto_id, archivo_id)
);

ALTER TABLE docs.ocr_extraccion
    ADD CONSTRAINT fk_ocr_gasto_soporte
    FOREIGN KEY (gasto_soporte_id) REFERENCES viaticos.gasto_soporte(id) ON DELETE SET NULL;

CREATE TABLE viaticos.legalizacion_historial (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    legalizacion_id UUID NOT NULL REFERENCES viaticos.legalizacion(id) ON DELETE CASCADE,
    estado_anterior viaticos.estado_legalizacion_enum,
    estado_nuevo    viaticos.estado_legalizacion_enum NOT NULL,
    usuario_id      UUID NOT NULL REFERENCES core.empleado(id),
    comentario      TEXT,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE viaticos.legalizacion IS 'Solicitud de legalización de viáticos';
COMMENT ON TABLE viaticos.gasto IS 'Gasto individual dentro de una legalización';
COMMENT ON TABLE viaticos.gasto_soporte IS 'Documento soporte adjunto a un gasto';
COMMENT ON TABLE viaticos.legalizacion_historial IS 'Historial de estados y comentarios del flujo';
