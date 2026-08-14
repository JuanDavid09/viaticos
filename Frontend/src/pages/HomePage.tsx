import { FileText, Inbox, ShieldCheck } from "lucide-react";
import { Topbar } from "@/components/layout/Topbar";

export function HomePage() {
  return (
    <>
      <Topbar title="Panel inicial" kicker="Viáticos MVP" />
      <main className="content">
        <div className="phase-banner">
          <div>
            <strong>Fase 0 lista para revisión</strong>
            <p className="page-lead" style={{ marginBottom: 0 }}>
              Esta pantalla define la identidad visual y la estructura. El login, las
              legalizaciones y las bandejas se conectarán al API en las siguientes fases.
            </p>
          </div>
          <span className="badge">Sin backend todavía</span>
        </div>

        <p className="page-lead">
          Plataforma interna para registrar viajes, adjuntar soportes y completar el flujo
          de aprobación hasta el cierre de nómina.
        </p>

        <section className="grid grid-3">
          <article className="card">
            <FileText size={22} />
            <h3>Empleado</h3>
            <p>Crea legalizaciones, agrega gastos y envía a validación.</p>
          </article>
          <article className="card">
            <ShieldCheck size={22} />
            <h3>Jefe aprobador</h3>
            <p>Revisa pendientes, aprueba o rechaza con comentario.</p>
          </article>
          <article className="card">
            <Inbox size={22} />
            <h3>Nómina</h3>
            <p>Cierra las legalizaciones aprobadas y deja el expediente listo.</p>
          </article>
        </section>
      </main>
    </>
  );
}
