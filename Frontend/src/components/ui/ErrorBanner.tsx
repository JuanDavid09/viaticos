import { RefreshCw } from "lucide-react";

type ErrorBannerProps = {
  message: string;
  onRetry?: () => void;
  retryLabel?: string;
};

export function ErrorBanner({
  message,
  onRetry,
  retryLabel = "Reintentar",
}: ErrorBannerProps) {
  return (
    <div className="error-banner" role="alert">
      <p>{message}</p>
      {onRetry ? (
        <button type="button" className="btn btn-ghost error-retry" onClick={onRetry}>
          <RefreshCw size={16} />
          {retryLabel}
        </button>
      ) : null}
    </div>
  );
}
