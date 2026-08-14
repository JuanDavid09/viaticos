-- ============================================================
-- Viáticos — Autenticación con contraseña (upgrade)
-- Ejecutar en BD existente si ya se instaló sin estas columnas.
-- ============================================================

ALTER TABLE core.empleado
    ADD COLUMN IF NOT EXISTS password_hash VARCHAR(255),
    ADD COLUMN IF NOT EXISTS must_change_password BOOLEAN NOT NULL DEFAULT FALSE;

COMMENT ON COLUMN core.empleado.password_hash IS 'Hash de contraseña (ASP.NET Identity PasswordHasher v3)';
COMMENT ON COLUMN core.empleado.must_change_password IS 'Obliga cambio de contraseña en el próximo acceso';

-- Contraseñas demo (solo desarrollo):
--   admin@empresa.com     → Admin123!    (must_change_password = false)
--   resto de usuarios seed → Cambiar123!  (must_change_password = true)

UPDATE core.empleado
SET
    password_hash = 'AQAAAAIAAYagAAAAEOIlBx7ep2xO7DTNLyv2NkQmGgElqUxSXnMh4X+kcL5qkNhmu+Gx3p82UH2w56+tEA==',
    must_change_password = FALSE
WHERE email = 'admin@empresa.com';

UPDATE core.empleado
SET
    password_hash = 'AQAAAAIAAYagAAAAEFpL9ts7ogubaSqBM3kZehH9ig5uM198eXEB+GN4YVIWpHUNzKoqJaYgzT14bJ8V9g==',
    must_change_password = TRUE
WHERE email IN (
    'nomina@empresa.com',
    'jefe@empresa.com',
    'empleado@empresa.com',
    'empleado2@empresa.com'
);
