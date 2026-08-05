-- ============================================================
-- Viáticos MVP — Seed Data
-- ============================================================

-- Monedas
INSERT INTO core.moneda (codigo_iso, nombre, simbolo) VALUES
    ('COP', 'Peso Colombiano', '$'),
    ('USD', 'Dólar Estadounidense', 'US$');

-- Categorías de gasto
INSERT INTO core.categoria_gasto (codigo, nombre, requiere_soporte) VALUES
    ('TRANSPORTE',   'Transporte',       TRUE),
    ('ALIMENTACION', 'Alimentación',     TRUE),
    ('HOSPEDAJE',    'Hospedaje',        TRUE),
    ('COMBUSTIBLE',  'Combustible',      TRUE),
    ('PEAJES',       'Peajes y parqueo', TRUE),
    ('OTROS',        'Otros gastos',     TRUE);

-- Usuarios demo (cadena jefe → empleado)
INSERT INTO core.empleado (codigo_empleado, email, nombre, apellido, departamento, rol)
VALUES ('ADM001', 'admin@empresa.com', 'Admin', 'Sistema', 'TI', 'ADMIN');

INSERT INTO core.empleado (codigo_empleado, email, nombre, apellido, departamento, rol)
VALUES ('NOM001', 'nomina@empresa.com', 'Ana', 'Nómina', 'Recursos Humanos', 'NOMINA');

INSERT INTO core.empleado (codigo_empleado, email, nombre, apellido, departamento, rol)
VALUES ('JEF001', 'jefe@empresa.com', 'Carlos', 'Gerente', 'Ventas', 'JEFE_APROBADOR');

INSERT INTO core.empleado (codigo_empleado, email, nombre, apellido, departamento, rol, jefe_id)
SELECT 'EMP001', 'empleado@empresa.com', 'María', 'Pérez', 'Ventas', 'EMPLEADO', e.id
FROM core.empleado e WHERE e.codigo_empleado = 'JEF001';

INSERT INTO core.empleado (codigo_empleado, email, nombre, apellido, departamento, rol, jefe_id)
SELECT 'EMP002', 'empleado2@empresa.com', 'Juan', 'López', 'Ventas', 'EMPLEADO', e.id
FROM core.empleado e WHERE e.codigo_empleado = 'JEF001';
