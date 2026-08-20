-- ============================================================
-- 012: Backfill jefe_id para empleados sin jefe
--
-- El flujo de aprobación filtra por core.empleado.jefe_id.
-- Empleados creados sin jefe no aparecen en la bandeja del jefe.
-- ============================================================

UPDATE core.empleado AS e
SET jefe_id = j.id,
    updated_at = NOW()
FROM core.empleado AS j
WHERE e.rol = 'EMPLEADO'
  AND e.jefe_id IS NULL
  AND e.activo = TRUE
  AND j.rol IN ('JEFE_APROBADOR', 'ADMIN')
  AND j.activo = TRUE
  AND j.id = (
      SELECT j2.id
      FROM core.empleado j2
      WHERE j2.rol = 'JEFE_APROBADOR'
        AND j2.activo = TRUE
      ORDER BY j2.created_at
      LIMIT 1
  );
