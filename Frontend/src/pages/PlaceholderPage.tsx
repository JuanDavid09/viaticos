import { Topbar } from "@/components/layout/Topbar";

type PlaceholderPageProps = {
  kicker: string;
  title: string;
  description: string;
  nextPhase: string;
};

export function PlaceholderPage({ kicker, title, description, nextPhase }: PlaceholderPageProps) {
  return (
    <>
      <Topbar title={title} kicker={kicker} />
      <main className="content">
        <div className="phase-banner">
          <div>
            <strong>Pantalla reservada</strong>
            <p className="page-lead" style={{ marginBottom: 0 }}>
              {description}
            </p>
          </div>
          <span className="badge">{nextPhase}</span>
        </div>
        <article className="card">
          <h3>Qué verás aquí más adelante</h3>
          <p>
            El layout, la navegación y los estilos ya están definidos. En la fase indicada
            esta vista se reemplazará por datos reales del API, sin cambiar la estructura
            general de la aplicación.
          </p>
        </article>
      </main>
    </>
  );
}
