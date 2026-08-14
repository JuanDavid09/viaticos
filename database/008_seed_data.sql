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
-- Contraseñas demo (solo desarrollo):
--   admin@empresa.com      → Admin123!
--   demás usuarios seed    → Cambiar123! (deben cambiar clave al ingresar)

INSERT INTO core.empleado (
    codigo_empleado, email, nombre, apellido, departamento, rol,
    password_hash, must_change_password
)
VALUES (
    'ADM001', 'admin@empresa.com', 'Admin', 'Sistema', 'TI', 'ADMIN',
    'AQAAAAIAAYagAAAAEOIlBx7ep2xO7DTNLyv2NkQmGgElqUxSXnMh4X+kcL5qkNhmu+Gx3p82UH2w56+tEA==',
    FALSE
);

INSERT INTO core.empleado (
    codigo_empleado, email, nombre, apellido, departamento, rol,
    password_hash, must_change_password
)
VALUES (
    'NOM001', 'nomina@empresa.com', 'Ana', 'Nómina', 'Recursos Humanos', 'NOMINA',
    'AQAAAAIAAYagAAAAEFpL9ts7ogubaSqBM3kZehH9ig5uM198eXEB+GN4YVIWpHUNzKoqJaYgzT14bJ8V9g==',
    TRUE
);

INSERT INTO core.empleado (
    codigo_empleado, email, nombre, apellido, departamento, rol,
    password_hash, must_change_password
)
VALUES (
    'JEF001', 'jefe@empresa.com', 'Carlos', 'Gerente', 'Ventas', 'JEFE_APROBADOR',
    'AQAAAAIAAYagAAAAEFpL9ts7ogubaSqBM3kZehH9ig5uM198eXEB+GN4YVIWpHUNzKoqJaYgzT14bJ8V9g==',
    TRUE
);

INSERT INTO core.empleado (
    codigo_empleado, email, nombre, apellido, departamento, rol, jefe_id,
    password_hash, must_change_password
)
SELECT
    'EMP001', 'empleado@empresa.com', 'María', 'Pérez', 'Ventas', 'EMPLEADO', e.id,
    'AQAAAAIAAYagAAAAEFpL9ts7ogubaSqBM3kZehH9ig5uM198eXEB+GN4YVIWpHUNzKoqJaYgzT14bJ8V9g==',
    TRUE
FROM core.empleado e WHERE e.codigo_empleado = 'JEF001';

INSERT INTO core.empleado (
    codigo_empleado, email, nombre, apellido, departamento, rol, jefe_id,
    password_hash, must_change_password
)
SELECT
    'EMP002', 'empleado2@empresa.com', 'Juan', 'López', 'Ventas', 'EMPLEADO', e.id,
    'AQAAAAIAAYagAAAAEFpL9ts7ogubaSqBM3kZehH9ig5uM198eXEB+GN4YVIWpHUNzKoqJaYgzT14bJ8V9g==',
    TRUE
FROM core.empleado e WHERE e.codigo_empleado = 'JEF001';
