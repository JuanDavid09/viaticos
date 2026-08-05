-- ============================================================
-- Viáticos MVP — Functions, Triggers and Views
-- ============================================================

-- updated_at automático
CREATE OR REPLACE FUNCTION core.fn_set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at := NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_empleado_updated_at
    BEFORE UPDATE ON core.empleado
    FOR EACH ROW EXECUTE FUNCTION core.fn_set_updated_at();

CREATE TRIGGER trg_legalizacion_updated_at
    BEFORE UPDATE ON viaticos.legalizacion
    FOR EACH ROW EXECUTE FUNCTION core.fn_set_updated_at();

CREATE TRIGGER trg_gasto_updated_at
    BEFORE UPDATE ON viaticos.gasto
    FOR EACH ROW EXECUTE FUNCTION core.fn_set_updated_at();

-- Número legible: LEG-2026-00001
CREATE OR REPLACE FUNCTION viaticos.fn_generar_numero_legalizacion()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.numero IS NULL OR NEW.numero = '' THEN
        NEW.numero := 'LEG-' || TO_CHAR(NOW(), 'YYYY') || '-' ||
                      LPAD(nextval('viaticos.legalizacion_numero_seq')::TEXT, 5, '0');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_legalizacion_numero
    BEFORE INSERT ON viaticos.legalizacion
    FOR EACH ROW EXECUTE FUNCTION viaticos.fn_generar_numero_legalizacion();

-- Historial automático al cambiar estado
CREATE OR REPLACE FUNCTION viaticos.fn_registrar_cambio_estado()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'UPDATE' AND OLD.estado IS DISTINCT FROM NEW.estado THEN
        INSERT INTO viaticos.legalizacion_historial (
            legalizacion_id, estado_anterior, estado_nuevo, usuario_id
        ) VALUES (
            NEW.id, OLD.estado, NEW.estado, COALESCE(NEW.updated_by, NEW.created_by)
        );
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_legalizacion_historial
    AFTER UPDATE OF estado ON viaticos.legalizacion
    FOR EACH ROW EXECUTE FUNCTION viaticos.fn_registrar_cambio_estado();

-- Recalcular totales al modificar gastos
CREATE OR REPLACE FUNCTION viaticos.fn_recalcular_totales()
RETURNS TRIGGER AS $$
DECLARE
    v_legalizacion_id UUID;
    v_total           NUMERIC(18,2);
    v_anticipo        NUMERIC(18,2);
BEGIN
    v_legalizacion_id := COALESCE(NEW.legalizacion_id, OLD.legalizacion_id);

    SELECT COALESCE(SUM(g.monto), 0) INTO v_total
    FROM viaticos.gasto g
    WHERE g.legalizacion_id = v_legalizacion_id
      AND g.deleted_at IS NULL;

    SELECT l.monto_anticipo INTO v_anticipo
    FROM viaticos.legalizacion l
    WHERE l.id = v_legalizacion_id;

    UPDATE viaticos.legalizacion
    SET total_gastos     = v_total,
        total_reembolso  = GREATEST(v_total - v_anticipo, 0),
        total_devolucion = GREATEST(v_anticipo - v_total, 0),
        updated_at       = NOW()
    WHERE id = v_legalizacion_id;

    RETURN COALESCE(NEW, OLD);
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_gasto_recalcular_insert
    AFTER INSERT ON viaticos.gasto
    FOR EACH ROW EXECUTE FUNCTION viaticos.fn_recalcular_totales();

CREATE TRIGGER trg_gasto_recalcular_update
    AFTER UPDATE OF monto, deleted_at ON viaticos.gasto
    FOR EACH ROW EXECUTE FUNCTION viaticos.fn_recalcular_totales();

CREATE TRIGGER trg_gasto_recalcular_delete
    AFTER DELETE ON viaticos.gasto
    FOR EACH ROW EXECUTE FUNCTION viaticos.fn_recalcular_totales();

-- Vistas de registros activos
CREATE OR REPLACE VIEW viaticos.v_legalizacion_activa AS
SELECT * FROM viaticos.legalizacion WHERE deleted_at IS NULL;

CREATE OR REPLACE VIEW viaticos.v_gasto_activo AS
SELECT * FROM viaticos.gasto WHERE deleted_at IS NULL;
