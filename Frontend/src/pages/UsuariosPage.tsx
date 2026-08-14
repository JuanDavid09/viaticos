import { useCallback, useEffect, useState, type FormEvent } from "react";
import { Topbar } from "@/components/layout/Topbar";
import { PasswordInput } from "@/components/ui/PasswordInput";
import { createEmpleado, listEmpleados, resetEmpleadoPassword, updateEmpleado } from "@/api/empleados";
import { getRoleLabel } from "@/features/auth/roleUtils";
import { ApiError, type UserRole } from "@/types/auth";
import type { Empleado } from "@/types/empleado";
import { roleOptions } from "@/types/empleado";

type FormState = {
  codigoEmpleado: string;
  email: string;
  nombre: string;
  apellido: string;
  departamento: string;
  rol: UserRole;
  passwordTemporal: string;
  jefeId: string;
};

const emptyForm: FormState = {
  codigoEmpleado: "",
  email: "",
  nombre: "",
  apellido: "",
  departamento: "",
  rol: "EMPLEADO",
  passwordTemporal: "",
  jefeId: "",
};

export function UsuariosPage() {
  const [empleados, setEmpleados] = useState<Empleado[]>([]);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const loadData = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const data = await listEmpleados(true);
      setEmpleados(data);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo cargar la lista de usuarios.");
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadData();
  }, [loadData]);

  async function handleCreate(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    setSuccess(null);

    try {
      await createEmpleado({
        codigoEmpleado: form.codigoEmpleado,
        email: form.email,
        nombre: form.nombre,
        apellido: form.apellido,
        rol: form.rol,
        passwordTemporal: form.passwordTemporal,
        departamento: form.departamento || undefined,
        jefeId: form.jefeId || undefined,
      });
      setForm(emptyForm);
      setSuccess("Usuario creado. Deberá cambiar la contraseña en su primer ingreso.");
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo crear el usuario.");
    } finally {
      setIsSubmitting(false);
    }
  }

  async function toggleActivo(empleado: Empleado) {
    setError(null);
    setSuccess(null);
    try {
      await updateEmpleado(empleado.id, {
        nombre: empleado.nombre,
        apellido: empleado.apellido,
        rol: empleado.rol,
        departamento: empleado.departamento ?? undefined,
        jefeId: empleado.jefeId,
        activo: !empleado.activo,
      });
      setSuccess(`Usuario ${empleado.activo ? "desactivado" : "activado"}.`);
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo actualizar el usuario.");
    }
  }

  async function handleResetPassword(empleado: Empleado) {
    const passwordTemporal = window.prompt(
      `Nueva contraseña temporal para ${empleado.nombreCompleto}:`,
      "Cambiar123!",
    );
    if (!passwordTemporal) return;

    setError(null);
    setSuccess(null);
    try {
      await resetEmpleadoPassword(empleado.id, { passwordTemporal });
      setSuccess(`Contraseña restablecida para ${empleado.nombreCompleto}. Deberá cambiarla al ingresar.`);
      await loadData();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo restablecer la contraseña.");
    }
  }

  const jefes = empleados.filter(
    (item) => item.rol === "JEFE_APROBADOR" && item.activo,
  );

  return (
    <>
      <Topbar title="Gestión de usuarios" kicker="Administración" />
      <main className="content">
        <p className="page-lead">
          Crea cuentas, asigna roles y restablece contraseñas temporales. Cada usuario nuevo debe
          cambiar su clave en el primer acceso.
        </p>

        {error ? <p className="login-error" role="alert">{error}</p> : null}
        {success ? <p className="success-banner">{success}</p> : null}

        <section className="grid grid-2 admin-layout">
          <article className="card card-form">
            <h3>Nuevo usuario</h3>
            <form className="stack-form" onSubmit={handleCreate}>
              <label>
                Código
                <input
                  value={form.codigoEmpleado}
                  onChange={(e) => setForm({ ...form, codigoEmpleado: e.target.value })}
                  required
                />
              </label>
              <label>
                Correo
                <input
                  type="email"
                  value={form.email}
                  onChange={(e) => setForm({ ...form, email: e.target.value })}
                  required
                />
              </label>
              <div className="form-row">
                <label>
                  Nombre
                  <input
                    value={form.nombre}
                    onChange={(e) => setForm({ ...form, nombre: e.target.value })}
                    required
                  />
                </label>
                <label>
                  Apellido
                  <input
                    value={form.apellido}
                    onChange={(e) => setForm({ ...form, apellido: e.target.value })}
                    required
                  />
                </label>
              </div>
              <label>
                Departamento
                <input
                  value={form.departamento}
                  onChange={(e) => setForm({ ...form, departamento: e.target.value })}
                />
              </label>
              <label>
                Rol
                <select
                  value={form.rol}
                  onChange={(e) => setForm({ ...form, rol: e.target.value as UserRole })}
                >
                  {roleOptions.map((option) => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </label>
              <label>
                Jefe (opcional)
                <select
                  value={form.jefeId}
                  onChange={(e) => setForm({ ...form, jefeId: e.target.value })}
                >
                  <option value="">Sin jefe</option>
                  {jefes.map((jefe) => (
                    <option key={jefe.id} value={jefe.id}>
                      {jefe.nombreCompleto}
                    </option>
                  ))}
                </select>
              </label>
              <label htmlFor="password-temporal">
                Contraseña temporal
                <PasswordInput
                  id="password-temporal"
                  value={form.passwordTemporal}
                  onChange={(e) => setForm({ ...form, passwordTemporal: e.target.value })}
                  required
                  minLength={8}
                />
              </label>
              <button className="btn btn-primary" type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Creando…" : "Crear usuario"}
              </button>
            </form>
          </article>

          <article className="card">
            <h3>Usuarios registrados</h3>
            {isLoading ? <p>Cargando usuarios…</p> : null}
            {!isLoading && empleados.length === 0 ? <p>No hay usuarios.</p> : null}
            <div className="table-list">
              {empleados.map((empleado) => (
                <div key={empleado.id} className="table-row">
                  <div>
                    <strong>{empleado.nombreCompleto}</strong>
                    <span className="table-meta">{empleado.email}</span>
                    <span className="table-meta">
                      {getRoleLabel(empleado.rol)}
                      {!empleado.activo ? " · Inactivo" : ""}
                      {empleado.mustChangePassword ? " · Debe cambiar clave" : ""}
                    </span>
                  </div>
                  <div className="table-actions">
                    <button
                      type="button"
                      className="btn btn-ghost"
                      onClick={() => void handleResetPassword(empleado)}
                    >
                      Restablecer clave
                    </button>
                    <button
                      type="button"
                      className="btn btn-ghost"
                      onClick={() => void toggleActivo(empleado)}
                    >
                      {empleado.activo ? "Desactivar" : "Activar"}
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </article>
        </section>
      </main>
    </>
  );
}
