import { env } from "@/config/env";

type LoginLogoProps = {
  variant?: "default" | "compact";
};

export function LoginLogo({ variant = "default" }: LoginLogoProps) {
  const isCompact = variant === "compact";

  return (
    <div className={`login-logo${isCompact ? " login-logo-compact" : ""}`}>
      <img
        src={env.logoUrl}
        alt={`Logotipo de ${env.appName}`}
        className="login-logo-image"
        onError={(event) => {
          const target = event.currentTarget;
          if (target.dataset.fallbackApplied === "true") return;
          target.dataset.fallbackApplied = "true";
          target.src = "/logo.svg";
        }}
      />
      {!isCompact ? (
        <div className="login-logo-text">
          <strong>{env.appName}</strong>
          <span>Legalización de viáticos</span>
        </div>
      ) : null}
    </div>
  );
}
