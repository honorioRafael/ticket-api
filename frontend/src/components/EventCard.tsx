import { Link } from "react-router-dom";
import { Event } from "@/types/domain";
import { formatBRL, getTicketTypesByEvent, getVenue } from "@/data/mock";
import { ArrowUpRight, MapPin } from "lucide-react";

export const EventCard = ({ event }: { event: Event }) => {
  const types = getTicketTypesByEvent(event.id);
  const minPrice = types.length ? Math.min(...types.map((t) => t.price)) : 0;
  const venue = getVenue(event.venueId);
  const date = new Date(event.startsAt);
  const dateLabel = date.toLocaleDateString("pt-BR", { day: "2-digit", month: "short" }).replace(".", "");
  const weekday = date.toLocaleDateString("pt-BR", { weekday: "long" });

  return (
    <Link
      to={`/evento/${event.id}`}
      className="group bg-surface ring-1 ring-border rounded-2xl p-3 flex flex-col transition-all hover:ring-foreground/20 hover:shadow-md"
    >
      <div className="relative aspect-[4/3] rounded-xl overflow-hidden bg-muted">
        <img
          src={event.imageUrl}
          alt={event.name}
          loading="lazy"
          className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
        />
        {event.category && (
          <span className="absolute top-3 left-3 bg-background/95 backdrop-blur text-xs font-medium px-2 py-1 rounded-md ring-1 ring-border">
            {event.category}
          </span>
        )}
      </div>
      <div className="mt-4 px-1 pb-2 flex flex-col flex-1">
        <p className="font-mono-feat text-xs text-muted-foreground uppercase tracking-wider">
          {dateLabel} · {weekday}
        </p>
        <h3 className="mt-1.5 text-base font-semibold leading-snug text-balance">{event.name}</h3>
        <p className="mt-2 text-sm text-muted-foreground flex items-center gap-1.5">
          <MapPin className="w-3.5 h-3.5" />
          {venue?.name}
        </p>
        <div className="mt-auto pt-5 flex items-center justify-between border-t border-border">
          <div className="flex flex-col">
            <span className="font-mono-feat text-[10px] text-muted-foreground uppercase font-medium">A partir de</span>
            <span className="text-sm font-semibold">{formatBRL(minPrice)}</span>
          </div>
          <span className="bg-primary text-primary-foreground text-sm font-medium py-2 pl-3 pr-2 rounded-lg flex items-center gap-1 transition-transform group-hover:translate-x-0.5">
            Comprar <ArrowUpRight className="w-4 h-4" />
          </span>
        </div>
      </div>
    </Link>
  );
};
