import { useNavigate, useParams } from "react-router-dom";
import { PageLayout } from "@/components/layout/PageLayout";
import { formatBRL, formatDateTime, getEvent, getTicketTypesByEvent, getVenue } from "@/data/mock";
import { Calendar, MapPin, Ticket, Users } from "lucide-react";
import { useMemo } from "react";

const EventDetail = () => {
  const { id = "" } = useParams();
  const navigate = useNavigate();
  const event = getEvent(id);
  const venue = event ? getVenue(event.venueId) : undefined;
  const types = useMemo(() => (event ? getTicketTypesByEvent(event.id) : []), [event]);

  if (!event) {
    return (
      <PageLayout>
        <div className="max-w-7xl mx-auto px-6 py-24 text-center">
          <h1 className="text-2xl font-semibold">Evento não encontrado</h1>
        </div>
      </PageLayout>
    );
  }

  const fromPrice = types.length ? Math.min(...types.map((t) => t.price)) : 0;

  return (
    <PageLayout>
      <div className="max-w-7xl mx-auto px-6 pt-10 pb-20">
        <div className="aspect-[16/7] rounded-2xl overflow-hidden bg-muted ring-1 ring-border">
          <img src={event.imageUrl} alt={event.name} className="w-full h-full object-cover" />
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-12 mt-10">
          <div className="lg:col-span-2">
            <span className="font-mono-feat text-xs uppercase tracking-widest text-muted-foreground">
              {event.category}
            </span>
            <h1 className="mt-3 text-3xl md:text-5xl font-semibold tracking-tight text-balance">{event.name}</h1>
            <p className="mt-6 text-base text-muted-foreground max-w-2xl text-pretty">{event.description}</p>

            <div className="mt-10 grid grid-cols-1 sm:grid-cols-2 gap-4">
              <InfoRow icon={<Calendar className="w-4 h-4" />} label="Data e horário" value={formatDateTime(event.startsAt)} />
              <InfoRow icon={<MapPin className="w-4 h-4" />} label="Local" value={`${venue?.name} — ${venue?.address}`} />
              <InfoRow icon={<Users className="w-4 h-4" />} label="Capacidade" value={`${venue?.capacity} pessoas`} />
              <InfoRow icon={<Ticket className="w-4 h-4" />} label="A partir de" value={formatBRL(fromPrice)} />
            </div>
          </div>

          <aside className="lg:sticky lg:top-24 self-start">
            <div className="bg-card ring-1 ring-border rounded-2xl p-6 shadow-md">
              <p className="font-mono-feat text-[10px] uppercase tracking-wider text-muted-foreground">A partir de</p>
              <p className="mt-1 text-3xl font-semibold">{formatBRL(fromPrice)}</p>
              <p className="mt-1 text-sm text-muted-foreground">{types.length} tipos de ingresso disponíveis</p>
              <button
                onClick={() => navigate(`/evento/${event.id}/ingressos`)}
                className="mt-6 w-full bg-primary text-primary-foreground text-sm font-medium py-3 rounded-lg hover:opacity-90 transition-opacity"
              >
                Comprar ingresso
              </button>
              <p className="mt-3 text-xs text-muted-foreground text-center">
                Você poderá escolher tipo e quantidade na próxima etapa.
              </p>
            </div>
          </aside>
        </div>
      </div>
    </PageLayout>
  );
};

const InfoRow = ({ icon, label, value }: { icon: React.ReactNode; label: string; value?: string }) => (
  <div className="flex items-start gap-3 p-4 rounded-xl bg-surface">
    <div className="mt-0.5 text-muted-foreground">{icon}</div>
    <div>
      <p className="font-mono-feat text-[10px] uppercase tracking-wider text-muted-foreground">{label}</p>
      <p className="text-sm font-medium mt-0.5">{value}</p>
    </div>
  </div>
);

export default EventDetail;
