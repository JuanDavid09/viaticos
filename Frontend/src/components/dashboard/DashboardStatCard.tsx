import type { ReactNode } from "react";

type DashboardStatCardProps = {
  label: string;
  value: number;
  tone?: "neutral" | "info" | "success" | "warning" | "danger";
  icon?: ReactNode;
};

export function DashboardStatCard({
  label,
  value,
  tone = "neutral",
  icon,
}: DashboardStatCardProps) {
  return (
    <article className={`dashboard-stat dashboard-stat-${tone}`}>
      <div className="dashboard-stat-header">
        <span className="dashboard-stat-label">{label}</span>
        {icon ? <span className="dashboard-stat-icon">{icon}</span> : null}
      </div>
      <strong className="dashboard-stat-value">{value}</strong>
    </article>
  );
}
