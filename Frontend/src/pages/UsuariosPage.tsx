import { useCallback, useEffect, useState, type FormEvent } from "react";
import { Topbar } from "@/components/layout/Topbar";
import { ResetPasswordModal } from "@/components/admin/ResetPasswordModal";
import { PasswordInput } from "@/components/ui/PasswordInput";
import { ConfirmDialog } from "@/components/ui/ConfirmDialog";
import { EmptyState } from "@/components/ui/EmptyState";
import { ErrorBanner } from "@/components/ui/ErrorBanner";
import { LoadingState } from "@/components/ui/LoadingState";
import { SuccessBanner } from "@/components/ui/SuccessBanner";
import { createEmpleado, listEmpleados, resetEmpleadoPassword, updateEmpleado } from "@/api/empleados";
import { getRoleLabel } from "@/features/auth/roleUtils";
import { getApiErrorMessage } from "@/lib/apiErrorMessage";
import type { UserRole } from "@/types/auth";
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
  const [resetTarget, setResetTarget] = useState<Empleado | null>(null);
  const [toggleTarget, setToggleTarget] = useState<Empleado | null>(null);

  const loadData = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const data = await listEmpleados(true);
      setEmpleados(data);
    } catch (err) {
      setError(getApiErrorMessage(err, "No se pudo cargar la lista de usuarios."));
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
      setSuccess("Usuario creado. Deberá cambiar la contraseña en su primer acceso.");
      await loadData();
    } catch (err) {
      setError(getApiErrorMessage(err, "No se pudo crear el usuario."));
    } finally {
      setIsSubmitting(false);
    }
  }

  async function confirmToggleActivo() {
    if (!toggleTarget) return;

    setError(null);
    setSuccess(null);
    try {
      await updateEmpleado(toggleTarget.id, {
        nombre: toggleTarget.nombre,
        apellido: toggleTarget.apellido,
        rol: toggleTarget.rol,
        departamento: toggleTarget.departamento ?? undefined,
        jefeId: toggleTarget.jefeId,
        activo: !toggleTarget.activo,
      });
      setSuccess(`Usuario ${toggleTarget.activo ? "desactivado" : "activado"}.`);
      setToggleTarget(null);
      await loadData();
    } catch (err) {
      setError(getApiErrorMessage(err, "No se pudo actualizar el usuario."));
    }
  }

  async function handleResetPassword(passwordTemporal: string) {
    if (!resetTarget) return;

    setIsSubmitting(true);
    setError(null);
    setSuccess(null);
    try {
      await resetEmpleadoPassword(resetTarget.id, { passwordTemporal });
      setSuccess(`Contraseña restablecida para ${resetTarget.nombreCompleto}. Deberá cambiarla al ingresar.`);
      setResetTarget(null);
      await loadData();
    } catch (err) {
      setError(getApiErrorMessage(err, "No se pudo restablecer la contraseña."));
    } finally {
      setIsSubmitting(false);
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

        {error ? <ErrorBanner message={error} onRetry={() => void loadData()} /> : null}
        {success ? <SuccessBanner message={success} onDismiss={() => setSuccess(null)} /> : null}

        <section className="grid grid-2 admin-layout">
          <article className="card card-form">
            <h3>Nuevo usuario</h3>
            <form className="stack-form" onSubmit={handleCreate}>
              <label htmlFor="usuario-codigo">
                Código
                <input
                  id="usuario-codigo"
                  value={form.codigoEmpleado}
                  onChange={(e) => setForm({ ...form, codigoEmpleado: e.target.value })}
                  required
                />
              </label>
              <label htmlFor="usuario-email">
                Correo
                <input
                  id="usuario-email"
                  type="email"
                  value={form.email}
                  onChange={(e) => setForm({ ...form, email: e.target.value })}
                  required
                />
              </label>
              <div className="form-row">
                <label htmlFor="usuario-nombre">
                  Nombre
                  <input
                    id="usuario-nombre"
                    value={form.nombre}
                    onChange={(e) => setForm({ ...form, nombre: e.target.value })}
                    required
                  />
                </label>
                <label htmlFor="usuario-apellido">
                  Apellido
                  <input
                    id="usuario-apellido"
                    value={form.apellido}
                    onChange={(e) => setForm({ ...form, apellido: e.target.value })}
                    required
                  />
                </label>
              </div>
              <label htmlFor="usuario-departamento">
                Departamento
                <input
                  id="usuario-departamento"
                  value={form.departamento}
                  onChange={(e) => setForm({ ...form, departamento: e.target.value })}
                />
              </label>
              <label htmlFor="usuario-rol">
                Rol
                <select
                  id="usuario-rol"
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
              <label htmlFor="usuario-jefe">
                Jefe (opcional)
                <select
                  id="usuario-jefe"
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
            {isLoading ? <LoadingState label="Cargando usuarios…" skeletonRows={4} /> : null}
            {!isLoading && empleados.length === 0 ? (
              <EmptyState
                title="No hay usuarios registrados"
                description="Crea el primer usuario con el formulario de la izquierda."
              />
            ) : null}
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
                      onClick={() => setResetTarget(empleado)}
                    >
                      Restablecer clave
                    </button>
                    <button
                      type="button"
                      className={`btn ${empleado.activo ? "btn-danger" : "btn-primary"}`}
                      onClick={() => setToggleTarget(empleado)}
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

      <ResetPasswordModal
        isOpen={resetTarget !== null}
        empleadoNombre={resetTarget?.nombreCompleto ?? ""}
        isSubmitting={isSubmitting}
        onClose={() => setResetTarget(null)}
        onSubmit={(password) => void handleResetPassword(password)}
      />

      <ConfirmDialog
        isOpen={toggleTarget !== null}
        title={toggleTarget?.activo ? "Desactivar usuario" : "Activar usuario"}
        message={
          toggleTarget?.activo
            ? `${toggleTarget.nombreCompleto} no podrá iniciar sesión hasta que lo actives de nuevo.`
            : `¿Confirmas que deseas activar a ${toggleTarget?.nombreCompleto}?`
        }
        confirmLabel={toggleTarget?.activo ? "Desactivar" : "Activar"}
        isDanger={toggleTarget?.activo ?? false}
        onClose={() => setToggleTarget(null)}
        onConfirm={() => void confirmToggleActivo()}
      />
    </>
  );
}
