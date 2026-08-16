import { useEffect } from "react";

type SuccessBannerProps = {
  message: string;
  onDismiss?: () => void;
  autoDismissMs?: number;
};

export function SuccessBanner({
  message,
  onDismiss,
  autoDismissMs = 5000,
}: SuccessBannerProps) {
  useEffect(() => {
    if (!onDismiss || autoDismissMs <= 0) return undefined;

    const timer = window.setTimeout(onDismiss, autoDismissMs);
    return () => window.clearTimeout(timer);
  }, [message, onDismiss, autoDismissMs]);

  return (
    <p className="success-banner" role="status">
      {message}
    </p>
  );
}
