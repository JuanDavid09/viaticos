import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";

export function LoginPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState("empleado@empresa.com");

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    navigate("/");
  }

  return (
    <div className="login-screen">
      <section className="login-panel">
        <p className="page-kicker">Acceso interno</p>
        <h1 className="page-title">Legalización de viáticos</h1>
        <p className="page-lead">
          En esta fase el formulario es solo visual. La autenticación real contra el API
          se conecta en la Fase 1.
        </p>
        <form className="login-form" onSubmit={handleSubmit}>
          <label htmlFor="email">Correo corporativo</label>
          <input
            id="email"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            placeholder="empleado@empresa.com"
          />
          <button className="btn btn-primary" type="submit">
            Continuar al entorno visual
          </button>
        </form>
        <p className="login-hint">Usuarios demo: empleado, jefe, nomina y admin @empresa.com</p>
      </section>
    </div>
  );
}
