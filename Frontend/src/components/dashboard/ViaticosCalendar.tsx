import { useCallback, useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { ChevronLeft, ChevronRight, ExternalLink } from "lucide-react";
import { listCalendarioLegalizaciones } from "@/api/legalizaciones";
import { appRoutes } from "@/app/routes";
import { EstadoBadge } from "@/components/legalizaciones/EstadoBadge";
import { ErrorBanner } from "@/components/ui/ErrorBanner";
import { LoadingState } from "@/components/ui/LoadingState";
import { EmptyState } from "@/components/ui/EmptyState";
import {
  buildCalendarDays,
  formatMonthLabel,
  getEventsForDay,
  getMonthRange,
  getSaldoAnticipo,
  getTodayIsoDate,
  getWeekdayLabels,
  parseIsoDate,
  shiftMonth,
} from "@/features/dashboard/calendarUtils";
import {
  formatDate,
  formatMoneyWithSymbol,
  getEstadoTone,
} from "@/features/legalizaciones/legalizacionUtils";
import { getApiErrorMessage } from "@/lib/apiErrorMessage";
import type { LegalizacionCalendario } from "@/types/legalizacion";

type ViaticosCalendarProps = {
  scopeLabel: string;
};

export function ViaticosCalendar({ scopeLabel }: ViaticosCalendarProps) {
  const today = getTodayIsoDate();
  const initial = parseIsoDate(today);
  const [year, setYear] = useState(initial.getFullYear());
  const [month, setMonth] = useState(initial.getMonth());
  const [selectedDate, setSelectedDate] = useState(today);
  const [events, setEvents] = useState<LegalizacionCalendario[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const monthRange = useMemo(() => getMonthRange(year, month), [year, month]);
  const calendarDays = useMemo(() => buildCalendarDays(year, month), [year, month]);
  const selectedEvents = useMemo(
    () => getEventsForDay(events, selectedDate),
    [events, selectedDate],
  );

  const loadEvents = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const data = await listCalendarioLegalizaciones(monthRange);
      setEvents(data);
    } catch (err) {
      setError(getApiErrorMessage(err, "No se pudo cargar el calendario de viáticos."));
    } finally {
      setIsLoading(false);
    }
  }, [monthRange.desde, monthRange.hasta]);

  useEffect(() => {
    void loadEvents();
  }, [loadEvents]);

  function handlePreviousMonth() {
    const next = shiftMonth(year, month, -1);
    setYear(next.year);
    setMonth(next.month);
  }

  function handleNextMonth() {
    const next = shiftMonth(year, month, 1);
    setYear(next.year);
    setMonth(next.month);
  }

  function handleToday() {
    const now = parseIsoDate(getTodayIsoDate());
    setYear(now.getFullYear());
    setMonth(now.getMonth());
    setSelectedDate(getTodayIsoDate());
  }

  return (
    <article className="card viaticos-calendar">
      <div className="viaticos-calendar-header">
        <div>
          <h3>Calendario de viáticos</h3>
          <p className="table-meta">
            Visualiza viajes programados del {scopeLabel}, anticipos y gastos registrados.
          </p>
        </div>

        <div className="viaticos-calendar-nav">
          <button
            type="button"
            className="btn btn-ghost viaticos-calendar-nav-btn"
            onClick={handlePreviousMonth}
            aria-label="Mes anterior"
          >
            <ChevronLeft size={18} />
          </button>
          <strong className="viaticos-calendar-month">{formatMonthLabel(year, month)}</strong>
          <button
            type="button"
            className="btn btn-ghost viaticos-calendar-nav-btn"
            onClick={handleNextMonth}
            aria-label="Mes siguiente"
          >
            <ChevronRight size={18} />
          </button>
          <button type="button" className="btn btn-ghost" onClick={handleToday}>
            Hoy
          </button>
        </div>
      </div>

      {error ? <ErrorBanner message={error} onRetry={() => void loadEvents()} /> : null}

      <div className="viaticos-calendar-body">
        <div className="viaticos-calendar-grid-wrap">
          {isLoading ? <LoadingState label="Cargando calendario…" skeletonRows={4} /> : null}

          {!isLoading ? (
            <>
              <div className="viaticos-calendar-weekdays">
                {getWeekdayLabels().map((label) => (
                  <span key={label}>{label}</span>
                ))}
              </div>

              <div className="viaticos-calendar-grid">
                {calendarDays.map((day) => {
                  const dayEvents = getEventsForDay(events, day.date);
                  const isSelected = day.date === selectedDate;
                  const isToday = day.date === today;

                  return (
                    <button
                      key={day.date}
                      type="button"
                      className={[
                        "viaticos-calendar-day",
                        !day.inMonth ? "is-outside" : "",
                        isSelected ? "is-selected" : "",
                        isToday ? "is-today" : "",
                        dayEvents.length > 0 ? "has-events" : "",
                      ]
                        .filter(Boolean)
                        .join(" ")}
                      onClick={() => setSelectedDate(day.date)}
                    >
                      <span className="viaticos-calendar-day-number">
                        {parseIsoDate(day.date).getDate()}
                      </span>
                      {dayEvents.length > 0 ? (
                        <span className="viaticos-calendar-day-events">
                          {dayEvents.slice(0, 3).map((event) => (
                            <span
                              key={event.id}
                              className={`viaticos-calendar-event-dot tone-${getEstadoTone(event.estado)}`}
                              title={`${event.empleadoNombre} · ${event.motivo}`}
                            />
                          ))}
                          {dayEvents.length > 3 ? (
                            <span className="viaticos-calendar-more">+{dayEvents.length - 3}</span>
                          ) : null}
                        </span>
                      ) : null}
                    </button>
                  );
                })}
              </div>
            </>
          ) : null}
        </div>

        <aside className="viaticos-calendar-detail">
          <div className="viaticos-calendar-detail-header">
            <h4>{formatDate(selectedDate)}</h4>
            <span className="dashboard-count">{selectedEvents.length}</span>
          </div>

          {selectedEvents.length === 0 ? (
            <EmptyState
              title="Sin viáticos este día"
              description="Selecciona otra fecha o cambia de mes para ver viajes programados."
            />
          ) : (
            <div className="viaticos-calendar-detail-list">
              {selectedEvents.map((event) => {
                const saldo = getSaldoAnticipo(event);

                return (
                  <div key={event.id} className="viaticos-calendar-event-card">
                    <div className="row-title">
                      <strong>{event.empleadoNombre}</strong>
                      <EstadoBadge estado={event.estado} />
                    </div>
                    <p className="viaticos-calendar-event-title">{event.motivo}</p>
                    <p className="table-meta">
                      {event.numero}
                      {event.destino ? ` · ${event.destino}` : ""}
                    </p>
                    <p className="table-meta">
                      {formatDate(event.fechaInicio)} → {formatDate(event.fechaFin)}
                    </p>

                    <dl className="viaticos-calendar-metrics">
                      <div>
                        <dt>Anticipo</dt>
                        <dd>{formatMoneyWithSymbol(event.montoAnticipo, event.monedaSimbolo)}</dd>
                      </div>
                      <div>
                        <dt>Gastos</dt>
                        <dd>{formatMoneyWithSymbol(event.totalGastos, event.monedaSimbolo)}</dd>
                      </div>
                      <div>
                        <dt>Saldo anticipo</dt>
                        <dd className={saldo < 0 ? "text-danger" : ""}>
                          {formatMoneyWithSymbol(saldo, event.monedaSimbolo)}
                        </dd>
                      </div>
                    </dl>

                    {event.totalReembolso > 0 || event.totalDevolucion > 0 ? (
                      <p className="table-meta">
                        {event.totalReembolso > 0
                          ? `Reembolso: ${formatMoneyWithSymbol(event.totalReembolso, event.monedaSimbolo)}`
                          : null}
                        {event.totalReembolso > 0 && event.totalDevolucion > 0 ? " · " : null}
                        {event.totalDevolucion > 0
                          ? `Devolución: ${formatMoneyWithSymbol(event.totalDevolucion, event.monedaSimbolo)}`
                          : null}
                      </p>
                    ) : null}

                    <Link
                      className="viaticos-calendar-event-link"
                      to={`${appRoutes.legalizaciones}/${event.id}`}
                    >
                      Ver legalización
                      <ExternalLink size={14} />
                    </Link>
                  </div>
                );
              })}
            </div>
          )}
        </aside>
      </div>
    </article>
  );
}
