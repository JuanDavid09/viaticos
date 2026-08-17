import { env } from "@/config/env";

type AppLogoProps = {
  variant?: "default" | "compact" | "sidebar";
  showText?: boolean;
  tagline?: string;
};

export function AppLogo({
  variant = "default",
  showText = true,
  tagline,
}: AppLogoProps) {
  const isCompact = variant === "compact";
  const isSidebar = variant === "sidebar";
  const resolvedTagline =
    tagline ??
    (isSidebar ? "Legalización interna" : "Legalización de viáticos");

  const className = [
    "app-logo",
    isCompact ? "app-logo-compact" : "",
    isSidebar ? "app-logo-sidebar" : "",
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <div className={className}>
      <img
        src={env.logoUrl}
        alt={`Logotipo de ${env.appName}`}
        className="app-logo-image"
        onError={(event) => {
          const target = event.currentTarget;
          if (target.dataset.fallbackApplied === "true") return;
          target.dataset.fallbackApplied = "true";
          target.src = "/logo.svg";
        }}
      />
      {showText && !isCompact ? (
        <div className="app-logo-text">
          <strong>{env.appName}</strong>
          <span>{resolvedTagline}</span>
        </div>
      ) : null}
    </div>
  );
}
