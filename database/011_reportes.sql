-- ============================================================
-- Viáticos — Procedimientos almacenados para reportes
--
-- PostgreSQL implementa reportes como funciones que retornan TABLE.
-- Uso típico:
--   SELECT * FROM reportes.sp_resumen_por_estado();
--   SELECT * FROM reportes.sp_legalizaciones_detalle(p_jefe_id := '...');
-- ============================================================

CREATE SCHEMA IF NOT EXISTS reportes;

COMMENT ON SCHEMA reportes IS 'Procedimientos y vistas para reportes operativos y de nómina';

-- Vista base reutilizable por los reportes
CREATE OR REPLACE VIEW reportes.v_legalizacion_enriquecida AS
SELECT
    l.id,
    l.numero,
    l.empleado_id,
    e.codigo_empleado,
    TRIM(e.nombre || ' ' || e.apellido) AS empleado_nombre,
    e.departamento,
    e.jefe_id,
    jefe.codigo_empleado AS jefe_codigo,
    TRIM(jefe.nombre || ' ' || jefe.apellido) AS jefe_nombre,
    l.motivo,
    l.destino,
    l.fecha_inicio,
    l.fecha_fin,
    l.moneda_id,
    m.codigo_iso AS moneda_codigo,
    m.simbolo AS moneda_simbolo,
    l.monto_anticipo,
    l.estado,
    l.total_gastos,
    l.total_reembolso,
    l.total_devolucion,
    l.observaciones,
    l.created_at,
    l.submitted_at,
    l.closed_at
FROM viaticos.legalizacion l
INNER JOIN core.empleado e ON e.id = l.empleado_id
INNER JOIN core.moneda m ON m.id = l.moneda_id
LEFT JOIN core.empleado jefe ON jefe.id = e.jefe_id
WHERE l.deleted_at IS NULL;

COMMENT ON VIEW reportes.v_legalizacion_enriquecida IS
    'Legalizaciones activas con datos de empleado, jefe y moneda para reportes';

-- ------------------------------------------------------------
-- 1. Resumen por estado (dashboard admin / gerencia)
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_resumen_por_estado(
    p_fecha_desde   DATE DEFAULT NULL,
    p_fecha_hasta   DATE DEFAULT NULL,
    p_empleado_id   UUID DEFAULT NULL,
    p_jefe_id       UUID DEFAULT NULL,
    p_departamento  VARCHAR DEFAULT NULL
)
RETURNS TABLE (
    estado              viaticos.estado_legalizacion_enum,
    cantidad            BIGINT,
    total_anticipos     NUMERIC(18,2),
    total_gastos        NUMERIC(18,2),
    total_reembolsos    NUMERIC(18,2),
    total_devoluciones  NUMERIC(18,2)
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        v.estado,
        COUNT(*)::BIGINT AS cantidad,
        COALESCE(SUM(v.monto_anticipo), 0) AS total_anticipos,
        COALESCE(SUM(v.total_gastos), 0) AS total_gastos,
        COALESCE(SUM(v.total_reembolso), 0) AS total_reembolsos,
        COALESCE(SUM(v.total_devolucion), 0) AS total_devoluciones
    FROM reportes.v_legalizacion_enriquecida v
    WHERE (p_fecha_desde IS NULL OR v.fecha_fin >= p_fecha_desde)
      AND (p_fecha_hasta IS NULL OR v.fecha_inicio <= p_fecha_hasta)
      AND (p_empleado_id IS NULL OR v.empleado_id = p_empleado_id)
      AND (p_jefe_id IS NULL OR v.jefe_id = p_jefe_id)
      AND (p_departamento IS NULL OR v.departamento ILIKE p_departamento)
    GROUP BY v.estado
    ORDER BY v.estado;
$$;

COMMENT ON FUNCTION reportes.sp_resumen_por_estado IS
    'Totales agrupados por estado con filtros opcionales de fechas, empleado, jefe y departamento';

-- ------------------------------------------------------------
-- 2. Detalle de legalizaciones (export / listado gerencial)
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_legalizaciones_detalle(
    p_fecha_desde   DATE DEFAULT NULL,
    p_fecha_hasta   DATE DEFAULT NULL,
    p_empleado_id   UUID DEFAULT NULL,
    p_jefe_id       UUID DEFAULT NULL,
    p_departamento  VARCHAR DEFAULT NULL,
    p_estado        viaticos.estado_legalizacion_enum DEFAULT NULL
)
RETURNS TABLE (
    id                  UUID,
    numero              VARCHAR,
    empleado_codigo     VARCHAR,
    empleado_nombre     TEXT,
    departamento        VARCHAR,
    jefe_nombre         TEXT,
    motivo              TEXT,
    destino             VARCHAR,
    fecha_inicio        DATE,
    fecha_fin           DATE,
    moneda_codigo       CHAR,
    moneda_simbolo      VARCHAR,
    monto_anticipo      NUMERIC(18,2),
    estado              viaticos.estado_legalizacion_enum,
    total_gastos        NUMERIC(18,2),
    total_reembolso     NUMERIC(18,2),
    total_devolucion    NUMERIC(18,2),
    saldo_anticipo      NUMERIC(18,2),
    created_at          TIMESTAMPTZ,
    submitted_at        TIMESTAMPTZ,
    closed_at           TIMESTAMPTZ
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        v.id,
        v.numero,
        v.codigo_empleado,
        v.empleado_nombre,
        v.departamento,
        v.jefe_nombre,
        v.motivo,
        v.destino,
        v.fecha_inicio,
        v.fecha_fin,
        v.moneda_codigo,
        v.moneda_simbolo,
        v.monto_anticipo,
        v.estado,
        v.total_gastos,
        v.total_reembolso,
        v.total_devolucion,
        (v.monto_anticipo - v.total_gastos) AS saldo_anticipo,
        v.created_at,
        v.submitted_at,
        v.closed_at
    FROM reportes.v_legalizacion_enriquecida v
    WHERE (p_fecha_desde IS NULL OR v.fecha_fin >= p_fecha_desde)
      AND (p_fecha_hasta IS NULL OR v.fecha_inicio <= p_fecha_hasta)
      AND (p_empleado_id IS NULL OR v.empleado_id = p_empleado_id)
      AND (p_jefe_id IS NULL OR v.jefe_id = p_jefe_id)
      AND (p_departamento IS NULL OR v.departamento ILIKE p_departamento)
      AND (p_estado IS NULL OR v.estado = p_estado)
    ORDER BY v.created_at DESC;
$$;

COMMENT ON FUNCTION reportes.sp_legalizaciones_detalle IS
    'Listado detallado de legalizaciones para exportación y consulta gerencial';

-- ------------------------------------------------------------
-- 3. Gastos agrupados por categoría
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_gastos_por_categoria(
    p_fecha_desde   DATE DEFAULT NULL,
    p_fecha_hasta   DATE DEFAULT NULL,
    p_empleado_id   UUID DEFAULT NULL,
    p_jefe_id       UUID DEFAULT NULL,
    p_departamento  VARCHAR DEFAULT NULL,
    p_estado        viaticos.estado_legalizacion_enum DEFAULT NULL
)
RETURNS TABLE (
    categoria_codigo    VARCHAR,
    categoria_nombre    VARCHAR,
    cantidad_gastos     BIGINT,
    total_monto         NUMERIC(18,2),
    promedio_monto      NUMERIC(18,2)
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        c.codigo AS categoria_codigo,
        c.nombre AS categoria_nombre,
        COUNT(g.id)::BIGINT AS cantidad_gastos,
        COALESCE(SUM(g.monto), 0) AS total_monto,
        COALESCE(AVG(g.monto), 0) AS promedio_monto
    FROM viaticos.gasto g
    INNER JOIN core.categoria_gasto c ON c.id = g.categoria_gasto_id
    INNER JOIN viaticos.legalizacion l ON l.id = g.legalizacion_id
    INNER JOIN core.empleado e ON e.id = l.empleado_id
    WHERE g.deleted_at IS NULL
      AND l.deleted_at IS NULL
      AND (p_fecha_desde IS NULL OR g.fecha_gasto >= p_fecha_desde)
      AND (p_fecha_hasta IS NULL OR g.fecha_gasto <= p_fecha_hasta)
      AND (p_empleado_id IS NULL OR l.empleado_id = p_empleado_id)
      AND (p_jefe_id IS NULL OR e.jefe_id = p_jefe_id)
      AND (p_departamento IS NULL OR e.departamento ILIKE p_departamento)
      AND (p_estado IS NULL OR l.estado = p_estado)
    GROUP BY c.codigo, c.nombre
    ORDER BY total_monto DESC, c.nombre;
$$;

COMMENT ON FUNCTION reportes.sp_gastos_por_categoria IS
    'Consolidado de gastos por categoría con filtros de periodo y ámbito organizacional';

-- ------------------------------------------------------------
-- 4. Detalle línea a línea de gastos (nómina / ERP)
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_gastos_detalle(
    p_fecha_desde   DATE DEFAULT NULL,
    p_fecha_hasta   DATE DEFAULT NULL,
    p_empleado_id   UUID DEFAULT NULL,
    p_jefe_id       UUID DEFAULT NULL,
    p_legalizacion_id UUID DEFAULT NULL,
    p_estado        viaticos.estado_legalizacion_enum DEFAULT NULL
)
RETURNS TABLE (
    legalizacion_numero VARCHAR,
    legalizacion_estado viaticos.estado_legalizacion_enum,
    empleado_codigo     VARCHAR,
    empleado_nombre     TEXT,
    departamento        VARCHAR,
    moneda_codigo       CHAR,
    gasto_id            UUID,
    categoria_codigo    VARCHAR,
    categoria_nombre    VARCHAR,
    fecha_gasto         DATE,
    descripcion         TEXT,
    proveedor           VARCHAR,
    numero_documento    VARCHAR,
    monto               NUMERIC(18,2),
    validado            BOOLEAN,
    cantidad_soportes   BIGINT
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        l.numero AS legalizacion_numero,
        l.estado AS legalizacion_estado,
        e.codigo_empleado,
        TRIM(e.nombre || ' ' || e.apellido) AS empleado_nombre,
        e.departamento,
        m.codigo_iso AS moneda_codigo,
        g.id AS gasto_id,
        c.codigo AS categoria_codigo,
        c.nombre AS categoria_nombre,
        g.fecha_gasto,
        g.descripcion,
        g.proveedor,
        g.numero_documento,
        g.monto,
        g.validado,
        (
            SELECT COUNT(*)
            FROM viaticos.gasto_soporte gs
            WHERE gs.gasto_id = g.id
        )::BIGINT AS cantidad_soportes
    FROM viaticos.gasto g
    INNER JOIN viaticos.legalizacion l ON l.id = g.legalizacion_id
    INNER JOIN core.empleado e ON e.id = l.empleado_id
    INNER JOIN core.moneda m ON m.id = l.moneda_id
    INNER JOIN core.categoria_gasto c ON c.id = g.categoria_gasto_id
    WHERE g.deleted_at IS NULL
      AND l.deleted_at IS NULL
      AND (p_fecha_desde IS NULL OR g.fecha_gasto >= p_fecha_desde)
      AND (p_fecha_hasta IS NULL OR g.fecha_gasto <= p_fecha_hasta)
      AND (p_empleado_id IS NULL OR l.empleado_id = p_empleado_id)
      AND (p_jefe_id IS NULL OR e.jefe_id = p_jefe_id)
      AND (p_legalizacion_id IS NULL OR l.id = p_legalizacion_id)
      AND (p_estado IS NULL OR l.estado = p_estado)
    ORDER BY l.numero, g.orden, g.fecha_gasto;
$$;

COMMENT ON FUNCTION reportes.sp_gastos_detalle IS
    'Detalle de gastos por línea para conciliación de nómina o integración ERP';

-- ------------------------------------------------------------
-- 5. Resumen financiero por empleado (nómina)
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_resumen_financiero_empleado(
    p_fecha_desde DATE DEFAULT NULL,
    p_fecha_hasta DATE DEFAULT NULL,
    p_jefe_id     UUID DEFAULT NULL,
    p_departamento VARCHAR DEFAULT NULL,
    p_solo_cerradas BOOLEAN DEFAULT TRUE
)
RETURNS TABLE (
    empleado_id         UUID,
    empleado_codigo     VARCHAR,
    empleado_nombre     TEXT,
    departamento        VARCHAR,
    moneda_codigo       CHAR,
    cantidad_legalizaciones BIGINT,
    total_anticipos     NUMERIC(18,2),
    total_gastos        NUMERIC(18,2),
    total_reembolsos    NUMERIC(18,2),
    total_devoluciones  NUMERIC(18,2)
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        v.empleado_id,
        v.codigo_empleado,
        v.empleado_nombre,
        v.departamento,
        v.moneda_codigo,
        COUNT(*)::BIGINT AS cantidad_legalizaciones,
        COALESCE(SUM(v.monto_anticipo), 0) AS total_anticipos,
        COALESCE(SUM(v.total_gastos), 0) AS total_gastos,
        COALESCE(SUM(v.total_reembolso), 0) AS total_reembolsos,
        COALESCE(SUM(v.total_devolucion), 0) AS total_devoluciones
    FROM reportes.v_legalizacion_enriquecida v
    WHERE (NOT p_solo_cerradas OR v.estado = 'CERRADA')
      AND (
            p_fecha_desde IS NULL
            OR COALESCE(v.closed_at::DATE, v.fecha_fin) >= p_fecha_desde
          )
      AND (
            p_fecha_hasta IS NULL
            OR COALESCE(v.closed_at::DATE, v.fecha_fin) <= p_fecha_hasta
          )
      AND (p_jefe_id IS NULL OR v.jefe_id = p_jefe_id)
      AND (p_departamento IS NULL OR v.departamento ILIKE p_departamento)
    GROUP BY
        v.empleado_id,
        v.codigo_empleado,
        v.empleado_nombre,
        v.departamento,
        v.moneda_codigo
    ORDER BY v.empleado_nombre, v.moneda_codigo;
$$;

COMMENT ON FUNCTION reportes.sp_resumen_financiero_empleado IS
    'Totales de anticipos, gastos, reembolsos y devoluciones agrupados por empleado';

-- ------------------------------------------------------------
-- 6. Pendientes de aprobación (bandeja jefe)
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_pendientes_aprobacion(
    p_jefe_id UUID DEFAULT NULL
)
RETURNS TABLE (
    id                  UUID,
    numero              VARCHAR,
    empleado_codigo     VARCHAR,
    empleado_nombre     TEXT,
    departamento        VARCHAR,
    motivo              TEXT,
    destino             VARCHAR,
    fecha_inicio        DATE,
    fecha_fin           DATE,
    moneda_codigo       CHAR,
    monto_anticipo      NUMERIC(18,2),
    total_gastos        NUMERIC(18,2),
    submitted_at        TIMESTAMPTZ,
    dias_pendientes     INTEGER
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        v.id,
        v.numero,
        v.codigo_empleado,
        v.empleado_nombre,
        v.departamento,
        v.motivo,
        v.destino,
        v.fecha_inicio,
        v.fecha_fin,
        v.moneda_codigo,
        v.monto_anticipo,
        v.total_gastos,
        v.submitted_at,
        CASE
            WHEN v.submitted_at IS NULL THEN NULL
            ELSE GREATEST(0, (CURRENT_DATE - v.submitted_at::DATE))::INTEGER
        END AS dias_pendientes
    FROM reportes.v_legalizacion_enriquecida v
    WHERE v.estado = 'PENDIENTE_APROBACION'
      AND (p_jefe_id IS NULL OR v.jefe_id = p_jefe_id)
    ORDER BY v.submitted_at NULLS LAST, v.created_at;
$$;

COMMENT ON FUNCTION reportes.sp_pendientes_aprobacion IS
    'Legalizaciones en bandeja de aprobación del jefe con días pendientes';

-- ------------------------------------------------------------
-- 7. Pendientes de nómina (bandeja cierre)
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_pendientes_nomina()
RETURNS TABLE (
    id                  UUID,
    numero              VARCHAR,
    empleado_codigo     VARCHAR,
    empleado_nombre     TEXT,
    departamento        VARCHAR,
    motivo              TEXT,
    moneda_codigo       CHAR,
    monto_anticipo      NUMERIC(18,2),
    total_gastos        NUMERIC(18,2),
    total_reembolso     NUMERIC(18,2),
    total_devolucion    NUMERIC(18,2),
    submitted_at        TIMESTAMPTZ
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        v.id,
        v.numero,
        v.codigo_empleado,
        v.empleado_nombre,
        v.departamento,
        v.motivo,
        v.moneda_codigo,
        v.monto_anticipo,
        v.total_gastos,
        v.total_reembolso,
        v.total_devolucion,
        v.submitted_at
    FROM reportes.v_legalizacion_enriquecida v
    WHERE v.estado = 'PENDIENTE_NOMINA'
    ORDER BY v.submitted_at NULLS LAST, v.created_at;
$$;

COMMENT ON FUNCTION reportes.sp_pendientes_nomina IS
    'Legalizaciones aprobadas pendientes de cierre por nómina';

-- ------------------------------------------------------------
-- 8. Legalizaciones cerradas en un periodo
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_legalizaciones_cerradas(
    p_fecha_desde DATE DEFAULT NULL,
    p_fecha_hasta DATE DEFAULT NULL,
    p_empleado_id UUID DEFAULT NULL,
    p_jefe_id     UUID DEFAULT NULL,
    p_departamento VARCHAR DEFAULT NULL
)
RETURNS TABLE (
    id                  UUID,
    numero              VARCHAR,
    empleado_codigo     VARCHAR,
    empleado_nombre     TEXT,
    departamento        VARCHAR,
    moneda_codigo       CHAR,
    monto_anticipo      NUMERIC(18,2),
    total_gastos        NUMERIC(18,2),
    total_reembolso     NUMERIC(18,2),
    total_devolucion    NUMERIC(18,2),
    closed_at           TIMESTAMPTZ
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        v.id,
        v.numero,
        v.codigo_empleado,
        v.empleado_nombre,
        v.departamento,
        v.moneda_codigo,
        v.monto_anticipo,
        v.total_gastos,
        v.total_reembolso,
        v.total_devolucion,
        v.closed_at
    FROM reportes.v_legalizacion_enriquecida v
    WHERE v.estado = 'CERRADA'
      AND (p_fecha_desde IS NULL OR v.closed_at::DATE >= p_fecha_desde)
      AND (p_fecha_hasta IS NULL OR v.closed_at::DATE <= p_fecha_hasta)
      AND (p_empleado_id IS NULL OR v.empleado_id = p_empleado_id)
      AND (p_jefe_id IS NULL OR v.jefe_id = p_jefe_id)
      AND (p_departamento IS NULL OR v.departamento ILIKE p_departamento)
    ORDER BY v.closed_at DESC NULLS LAST;
$$;

COMMENT ON FUNCTION reportes.sp_legalizaciones_cerradas IS
    'Legalizaciones cerradas por nómina en un rango de fechas de cierre';

-- ------------------------------------------------------------
-- 9. Calendario de viáticos (jefe / admin)
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_calendario_viaticos(
    p_desde   DATE,
    p_hasta   DATE,
    p_jefe_id UUID DEFAULT NULL
)
RETURNS TABLE (
    id                  UUID,
    numero              VARCHAR,
    empleado_id         UUID,
    empleado_nombre     TEXT,
    departamento        VARCHAR,
    motivo              TEXT,
    destino             VARCHAR,
    fecha_inicio        DATE,
    fecha_fin           DATE,
    estado              viaticos.estado_legalizacion_enum,
    moneda_simbolo      VARCHAR,
    monto_anticipo      NUMERIC(18,2),
    total_gastos        NUMERIC(18,2),
    total_reembolso     NUMERIC(18,2),
    total_devolucion    NUMERIC(18,2)
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        v.id,
        v.numero,
        v.empleado_id,
        v.empleado_nombre,
        v.departamento,
        v.motivo,
        v.destino,
        v.fecha_inicio,
        v.fecha_fin,
        v.estado,
        v.moneda_simbolo,
        v.monto_anticipo,
        v.total_gastos,
        v.total_reembolso,
        v.total_devolucion
    FROM reportes.v_legalizacion_enriquecida v
    WHERE v.fecha_inicio <= p_hasta
      AND v.fecha_fin >= p_desde
      AND (p_jefe_id IS NULL OR v.jefe_id = p_jefe_id)
    ORDER BY v.fecha_inicio, v.empleado_nombre;
$$;

COMMENT ON FUNCTION reportes.sp_calendario_viaticos IS
    'Viajes que intersectan un rango de fechas; filtrable por jefe directo';

-- ------------------------------------------------------------
-- 10. Gastos sin soporte (cumplimiento documental)
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_gastos_sin_soporte(
    p_fecha_desde DATE DEFAULT NULL,
    p_fecha_hasta DATE DEFAULT NULL,
    p_estado      viaticos.estado_legalizacion_enum DEFAULT NULL
)
RETURNS TABLE (
    legalizacion_numero VARCHAR,
    legalizacion_estado viaticos.estado_legalizacion_enum,
    empleado_nombre     TEXT,
    categoria_nombre    VARCHAR,
    fecha_gasto         DATE,
    descripcion         TEXT,
    monto               NUMERIC(18,2),
    requiere_soporte    BOOLEAN
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        l.numero AS legalizacion_numero,
        l.estado AS legalizacion_estado,
        TRIM(e.nombre || ' ' || e.apellido) AS empleado_nombre,
        c.nombre AS categoria_nombre,
        g.fecha_gasto,
        g.descripcion,
        g.monto,
        c.requiere_soporte
    FROM viaticos.gasto g
    INNER JOIN viaticos.legalizacion l ON l.id = g.legalizacion_id
    INNER JOIN core.empleado e ON e.id = l.empleado_id
    INNER JOIN core.categoria_gasto c ON c.id = g.categoria_gasto_id
    WHERE g.deleted_at IS NULL
      AND l.deleted_at IS NULL
      AND c.requiere_soporte = TRUE
      AND NOT EXISTS (
            SELECT 1
            FROM viaticos.gasto_soporte gs
            WHERE gs.gasto_id = g.id
          )
      AND (p_fecha_desde IS NULL OR g.fecha_gasto >= p_fecha_desde)
      AND (p_fecha_hasta IS NULL OR g.fecha_gasto <= p_fecha_hasta)
      AND (p_estado IS NULL OR l.estado = p_estado)
    ORDER BY l.numero, g.fecha_gasto;
$$;

COMMENT ON FUNCTION reportes.sp_gastos_sin_soporte IS
    'Gastos cuya categoría exige soporte pero no tienen documento adjunto';

-- ------------------------------------------------------------
-- 11. Historial de auditoría del flujo
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_historial_auditoria(
    p_fecha_desde     DATE DEFAULT NULL,
    p_fecha_hasta     DATE DEFAULT NULL,
    p_legalizacion_id UUID DEFAULT NULL,
    p_empleado_id     UUID DEFAULT NULL
)
RETURNS TABLE (
    historial_id        UUID,
    legalizacion_numero VARCHAR,
    empleado_nombre     TEXT,
    estado_anterior     viaticos.estado_legalizacion_enum,
    estado_nuevo        viaticos.estado_legalizacion_enum,
    usuario_nombre      TEXT,
    comentario          TEXT,
    created_at          TIMESTAMPTZ
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        h.id AS historial_id,
        l.numero AS legalizacion_numero,
        TRIM(emp.nombre || ' ' || emp.apellido) AS empleado_nombre,
        h.estado_anterior,
        h.estado_nuevo,
        TRIM(usr.nombre || ' ' || usr.apellido) AS usuario_nombre,
        h.comentario,
        h.created_at
    FROM viaticos.legalizacion_historial h
    INNER JOIN viaticos.legalizacion l ON l.id = h.legalizacion_id
    INNER JOIN core.empleado emp ON emp.id = l.empleado_id
    INNER JOIN core.empleado usr ON usr.id = h.usuario_id
    WHERE l.deleted_at IS NULL
      AND (p_fecha_desde IS NULL OR h.created_at::DATE >= p_fecha_desde)
      AND (p_fecha_hasta IS NULL OR h.created_at::DATE <= p_fecha_hasta)
      AND (p_legalizacion_id IS NULL OR h.legalizacion_id = p_legalizacion_id)
      AND (p_empleado_id IS NULL OR l.empleado_id = p_empleado_id)
    ORDER BY h.created_at DESC;
$$;

COMMENT ON FUNCTION reportes.sp_historial_auditoria IS
    'Trazabilidad de cambios de estado con usuario, comentario y timestamps';

-- ------------------------------------------------------------
-- 12. Volumen y montos mensuales (gerencia)
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_volumen_mensual(
    p_anio        INTEGER DEFAULT NULL,
    p_jefe_id     UUID DEFAULT NULL,
    p_departamento VARCHAR DEFAULT NULL
)
RETURNS TABLE (
    anio                INTEGER,
    mes                 INTEGER,
    periodo             TEXT,
    cantidad_legalizaciones BIGINT,
    total_anticipos     NUMERIC(18,2),
    total_gastos        NUMERIC(18,2),
    total_reembolsos    NUMERIC(18,2),
    total_devoluciones  NUMERIC(18,2),
    cantidad_cerradas   BIGINT
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        EXTRACT(YEAR FROM v.created_at)::INTEGER AS anio,
        EXTRACT(MONTH FROM v.created_at)::INTEGER AS mes,
        TO_CHAR(v.created_at, 'YYYY-MM') AS periodo,
        COUNT(*)::BIGINT AS cantidad_legalizaciones,
        COALESCE(SUM(v.monto_anticipo), 0) AS total_anticipos,
        COALESCE(SUM(v.total_gastos), 0) AS total_gastos,
        COALESCE(SUM(v.total_reembolso), 0) AS total_reembolsos,
        COALESCE(SUM(v.total_devolucion), 0) AS total_devoluciones,
        COUNT(*) FILTER (WHERE v.estado = 'CERRADA')::BIGINT AS cantidad_cerradas
    FROM reportes.v_legalizacion_enriquecida v
    WHERE (p_anio IS NULL OR EXTRACT(YEAR FROM v.created_at) = p_anio)
      AND (p_jefe_id IS NULL OR v.jefe_id = p_jefe_id)
      AND (p_departamento IS NULL OR v.departamento ILIKE p_departamento)
    GROUP BY
        EXTRACT(YEAR FROM v.created_at),
        EXTRACT(MONTH FROM v.created_at),
        TO_CHAR(v.created_at, 'YYYY-MM')
    ORDER BY anio DESC, mes DESC;
$$;

COMMENT ON FUNCTION reportes.sp_volumen_mensual IS
    'Tendencia mensual de legalizaciones y montos agregados';

-- ------------------------------------------------------------
-- 13. Tiempo promedio en cada estado (SLA operativo)
-- ------------------------------------------------------------
CREATE OR REPLACE FUNCTION reportes.sp_tiempos_por_estado(
    p_fecha_desde DATE DEFAULT NULL,
    p_fecha_hasta DATE DEFAULT NULL
)
RETURNS TABLE (
    legalizacion_numero VARCHAR,
    empleado_nombre     TEXT,
    estado              viaticos.estado_legalizacion_enum,
    inicio_estado       TIMESTAMPTZ,
    fin_estado          TIMESTAMPTZ,
    horas_en_estado     NUMERIC(12,2)
)
LANGUAGE sql
STABLE
AS $$
    WITH transiciones AS (
        SELECT
            h.legalizacion_id,
            l.numero AS legalizacion_numero,
            TRIM(e.nombre || ' ' || e.apellido) AS empleado_nombre,
            h.estado_nuevo AS estado,
            h.created_at AS inicio_estado,
            LEAD(h.created_at) OVER (
                PARTITION BY h.legalizacion_id
                ORDER BY h.created_at
            ) AS fin_estado
        FROM viaticos.legalizacion_historial h
        INNER JOIN viaticos.legalizacion l ON l.id = h.legalizacion_id
        INNER JOIN core.empleado e ON e.id = l.empleado_id
        WHERE l.deleted_at IS NULL
    )
    SELECT
        t.legalizacion_numero,
        t.empleado_nombre,
        t.estado,
        t.inicio_estado,
        t.fin_estado,
        ROUND(
            EXTRACT(EPOCH FROM (COALESCE(t.fin_estado, NOW()) - t.inicio_estado)) / 3600.0,
            2
        ) AS horas_en_estado
    FROM transiciones t
    WHERE (p_fecha_desde IS NULL OR t.inicio_estado::DATE >= p_fecha_desde)
      AND (p_fecha_hasta IS NULL OR t.inicio_estado::DATE <= p_fecha_hasta)
    ORDER BY t.inicio_estado DESC;
$$;

COMMENT ON FUNCTION reportes.sp_tiempos_por_estado IS
    'Duración en horas de cada transición de estado para medir tiempos de atención';
