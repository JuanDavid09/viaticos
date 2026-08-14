type TopbarProps = {
  title: string;
  kicker?: string;
};

export function Topbar({ title, kicker }: TopbarProps) {
  return (
    <header className="topbar">
      <div>
        {kicker ? <p className="page-kicker">{kicker}</p> : null}
        <h2 className="page-title" style={{ marginBottom: 0, fontSize: "1.15rem" }}>
          {title}
        </h2>
      </div>
      <div className="user-chip" title="El usuario real se conectará en la Fase 1">
        <span className="avatar">MVP</span>
        <span>Sesión demo</span>
      </div>
    </header>
  );
}
