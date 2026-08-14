# Autenticación con contraseña — Backend

Extensión del login MVP: contraseñas hasheadas, cambio obligatorio en primer acceso y gestión de usuarios por Admin.

## Base de datos

### Columnas nuevas en `core.empleado`

| Columna | Tipo | Descripción |
|---------|------|-------------|
| `password_hash` | `VARCHAR(255)` | Hash ASP.NET Identity PasswordHasher v3 |
| `must_change_password` | `BOOLEAN` | Obliga cambio de clave en el próximo acceso |

### Scripts

- Instalación nueva: incluido en `database/003_core.sql` y `database/008_seed_data.sql`
- BD existente: ejecutar `database/009_auth_password.sql`

### Contraseñas demo (solo desarrollo)

| Usuario | Contraseña | Cambio obligatorio |
|---------|------------|--------------------|
| admin@empresa.com | Admin123! | No |
| demás usuarios seed | Cambiar123! | Sí |

## API

### Auth

```http
POST /api/auth/login
{ "email": "...", "password": "..." }

→ 200 { accessToken, expiresAt, userId, email, rol, nombreCompleto, mustChangePassword }
→ 401 { code: "INVALID_CREDENTIALS" }
```

```http
POST /api/auth/change-password
Authorization: Bearer ...
{ "currentPassword": "...", "newPassword": "..." }

→ 200 LoginResponse (JWT actualizado, mustChangePassword: false)
```

### Empleados (solo Admin)

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/empleados?includeInactive=false` | Listar usuarios |
| GET | `/api/empleados/{id}` | Detalle |
| POST | `/api/empleados` | Crear con contraseña temporal |
| PUT | `/api/empleados/{id}` | Actualizar perfil, rol, jefe, activo |
| POST | `/api/empleados/{id}/restablecer-password` | Nueva clave temporal + must_change_password |

### Bloqueo por cambio de clave pendiente

Si el JWT incluye `must_change_password: true`, el middleware rechaza cualquier endpoint excepto `POST /api/auth/change-password`:

```json
{ "code": "MUST_CHANGE_PASSWORD", "message": "Debe cambiar su contraseña antes de continuar." }
```

## Flujo operativo

1. **Admin** crea usuario con contraseña temporal (`must_change_password = true`).
2. **Usuario** inicia sesión con email + contraseña temporal.
3. Frontend redirige a `/cambiar-clave`; backend bloquea el resto de endpoints.
4. Usuario define contraseña personal (mín. 8 caracteres, mayúscula, minúscula, número).
5. API retorna JWT nuevo sin restricción.

## Checklist de verificación

- [ ] Ejecutar `database/009_auth_password.sql` en BD existente
- [ ] `POST /api/auth/login` falla sin contraseña o con credenciales incorrectas
- [ ] Empleado seed entra con `Cambiar123!` y recibe `mustChangePassword: true`
- [ ] Tras cambiar clave, puede acceder a catálogos/legalizaciones
- [ ] Admin entra con `Admin123!` y puede listar/crear usuarios en `/api/empleados`
- [ ] Usuario creado por admin debe cambiar clave en primer ingreso
