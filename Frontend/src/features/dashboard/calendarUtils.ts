import type { LegalizacionCalendario } from "@/types/legalizacion";

export type CalendarDay = {
  date: string;
  inMonth: boolean;
};

const WEEKDAY_LABELS = ["Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom"];

export function getWeekdayLabels(): string[] {
  return WEEKDAY_LABELS;
}

export function toIsoDate(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

export function parseIsoDate(value: string): Date {
  const [year, month, day] = value.split("-").map(Number);
  return new Date(year, month - 1, day);
}

export function getMonthRange(year: number, month: number) {
  return {
    desde: toIsoDate(new Date(year, month, 1)),
    hasta: toIsoDate(new Date(year, month + 1, 0)),
  };
}

export function buildCalendarDays(year: number, month: number): CalendarDay[] {
  const firstDay = new Date(year, month, 1);
  let startOffset = firstDay.getDay() - 1;
  if (startOffset < 0) startOffset = 6;

  const gridStart = new Date(year, month, 1 - startOffset);
  const days: CalendarDay[] = [];

  for (let index = 0; index < 42; index += 1) {
    const current = new Date(gridStart);
    current.setDate(gridStart.getDate() + index);
    days.push({
      date: toIsoDate(current),
      inMonth: current.getMonth() === month,
    });
  }

  return days;
}

export function formatMonthLabel(year: number, month: number): string {
  const label = new Intl.DateTimeFormat("es-CO", {
    month: "long",
    year: "numeric",
  }).format(new Date(year, month, 1));

  return label.charAt(0).toUpperCase() + label.slice(1);
}

export function isDateWithinRange(day: string, start: string, end: string): boolean {
  return day >= start && day <= end;
}

export function getEventsForDay(
  events: LegalizacionCalendario[],
  day: string,
): LegalizacionCalendario[] {
  return events
    .filter((event) => isDateWithinRange(day, event.fechaInicio, event.fechaFin))
    .sort((a, b) => a.empleadoNombre.localeCompare(b.empleadoNombre, "es"));
}

export function getTodayIsoDate(): string {
  return toIsoDate(new Date());
}

export function shiftMonth(year: number, month: number, delta: number) {
  const date = new Date(year, month + delta, 1);
  return { year: date.getFullYear(), month: date.getMonth() };
}

export function getSaldoAnticipo(event: LegalizacionCalendario): number {
  return event.montoAnticipo - event.totalGastos;
}
