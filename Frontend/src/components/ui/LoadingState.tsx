type LoadingStateProps = {
  label?: string;
  skeletonRows?: number;
};

export function LoadingState({
  label = "Cargando…",
  skeletonRows = 0,
}: LoadingStateProps) {
  return (
    <div className="loading-state" role="status" aria-live="polite">
      <div className="loading-spinner" aria-hidden="true" />
      <span>{label}</span>
      {skeletonRows > 0 ? (
        <div className="skeleton-list" aria-hidden="true">
          {Array.from({ length: skeletonRows }, (_, index) => (
            <div key={index} className="skeleton-row" />
          ))}
        </div>
      ) : null}
    </div>
  );
}
