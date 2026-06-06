import { PageLayout } from "@/components/layout/PageLayout";
import { events, formatBRL, formatDateTime, getEvent, getTicketType, orders, tickets } from "@/data/mock";
import { Ticket as TicketIcon } from "lucide-react";
import { Link } from "react-router-dom";

const MyTickets = () => {
  if (orders.length === 0) {
    return (
      <PageLayout>
        <div className="max-w-3xl mx-auto px-6 py-24 text-center">
          <div className="inline-grid place-items-center w-14 h-14 rounded-full bg-surface">
            <TicketIcon className="w-6 h-6 text-muted-foreground" />
          </div>
          <h1 className="mt-6 text-2xl font-semibold">Nenhum ingresso ainda</h1>
          <p className="mt-2 text-muted-foreground">Quando você comprar, eles aparecem aqui.</p>
          <Link to="/" className="mt-6 inline-block bg-primary text-primary-foreground px-5 py-3 rounded-lg text-sm font-medium">
            Ver eventos
          </Link>
        </div>
      </PageLayout>
    );
  }

  return (
    <PageLayout>
      <div className="max-w-5xl mx-auto px-6 py-12">
        <p className="font-mono-feat text-xs uppercase tracking-widest text-muted-foreground">// Conta</p>
        <h1 className="mt-2 text-3xl md:text-4xl font-semibold tracking-tight">Meus ingressos</h1>

        <div className="mt-10 space-y-6">
          {orders.map((order) => {
            const eventId = events.find((e) =>
              order.items.some((i) => getTicketType(i.ticketTypeId)?.eventId === e.id)
            )?.id;
            const event = eventId ? getEvent(eventId) : undefined;
            const ticketsOfOrder = tickets.filter((t) => order.items.some((i) => i.id === t.orderItemId));
            return (
              <div key={order.id} className="bg-card ring-1 ring-border rounded-2xl overflow-hidden">
                <div className="flex flex-col md:flex-row gap-4 p-5 border-b border-border">
                  {event?.imageUrl && (
                    <img src={event.imageUrl} alt="" className="w-full md:w-48 aspect-video md:aspect-square object-cover rounded-xl" />
                  )}
                  <div className="flex-1">
                    <p className="font-mono-feat text-[10px] uppercase text-muted-foreground">{order.id}</p>
                    <h2 className="mt-1 text-lg font-semibold">{event?.name}</h2>
                    <p className="text-sm text-muted-foreground mt-1">{event && formatDateTime(event.startsAt)}</p>
                    <div className="mt-3 flex gap-3 text-sm">
                      <span className="px-2 py-0.5 bg-surface rounded-md font-mono-feat text-xs uppercase">
                        {order.status === "confirmed" ? "Confirmado" : order.status}
                      </span>
                      <span className="text-muted-foreground">{formatBRL(order.totalAmount)}</span>
                    </div>
                  </div>
                </div>
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-2 p-5 bg-surface/50">
                  {ticketsOfOrder.map((t) => {
                    const item = order.items.find((i) => i.id === t.orderItemId)!;
                    const tt = getTicketType(item.ticketTypeId);
                    return (
                      <div key={t.id} className="bg-card rounded-lg p-3 border border-border flex justify-between items-center">
                        <div>
                          <p className="font-mono-feat text-[10px] uppercase text-muted-foreground">{tt?.name}</p>
                          <p className="font-mono-feat text-sm font-medium">{t.code}</p>
                        </div>
                        <span className="text-xs px-2 py-0.5 bg-primary text-primary-foreground rounded-md">{t.status}</span>
                      </div>
                    );
                  })}
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </PageLayout>
  );
};

export default MyTickets;
