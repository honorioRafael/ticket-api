import { PageLayout } from "@/components/layout/PageLayout";
import { formatBRL, formatDateTime } from "@/data/mock";
import { useAuth } from "@/store/auth";
import { salesApi, eventsApi } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import { Ticket as TicketIcon } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";

const MyTickets = () => {
  const navigate = useNavigate();
  const { user } = useAuth();

  // Fetch all orders for this customer from Sales API
  const { data: orders = [], isLoading: ordersLoading } = useQuery({
    queryKey: ["orders", user?.id],
    queryFn: () => salesApi.getOrders(),
    enabled: !!user,
  });

  // Fetch all events from Events API to map event metadata and ticket type names
  const { data: events = [], isLoading: eventsLoading } = useQuery({
    queryKey: ["events"],
    queryFn: () => eventsApi.getAll(),
    enabled: !!user,
  });

  if (!user) {
    return (
      <PageLayout>
        <div className="max-w-md mx-auto px-6 py-24 text-center">
          <h1 className="text-2xl font-semibold">Identificação necessária</h1>
          <p className="text-muted-foreground mt-2">Você precisa entrar na sua conta para visualizar seus ingressos.</p>
          <button
            onClick={() => navigate("/login?redirect=/meus-ingressos")}
            className="mt-6 w-full bg-primary text-primary-foreground py-3 rounded-lg text-sm font-medium hover:opacity-90 transition-opacity"
          >
            Entrar ou Criar conta
          </button>
        </div>
      </PageLayout>
    );
  }

  if (ordersLoading || eventsLoading) {
    return (
      <PageLayout>
        <div className="max-w-3xl mx-auto px-6 py-24 text-center">
          <h1 className="text-xl font-medium text-muted-foreground">Carregando seus ingressos...</h1>
        </div>
      </PageLayout>
    );
  }

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
            // Find event and ticket type by scanning all events
            let event: any = null;
            let firstTicketTypeName = "Ingresso";
            
            if (order.items && order.items.length > 0) {
              const firstItem = order.items[0];
              event = events.find((e) => 
                (e as any).ticketTypes?.some((t: any) => t.id === firstItem.ticketTypeId)
              );
              
              if (event) {
                const tt = (event as any).ticketTypes.find((t: any) => t.id === firstItem.ticketTypeId);
                if (tt) firstTicketTypeName = tt.name;
              }
            }

            return (
              <div key={order.id} className="bg-card ring-1 ring-border rounded-2xl overflow-hidden">
                <div className="flex flex-col md:flex-row gap-4 p-5 border-b border-border">
                  {event?.imageUrl && (
                    <img src={event.imageUrl} alt="" className="w-full md:w-48 aspect-video md:aspect-square object-cover rounded-xl" />
                  )}
                  <div className="flex-1">
                    <p className="font-mono-feat text-[10px] uppercase text-muted-foreground">Pedido: {order.id}</p>
                    <h2 className="mt-1 text-lg font-semibold">{event?.name || "Evento não identificado"}</h2>
                    <p className="text-sm text-muted-foreground mt-1">
                      {event ? formatDateTime(event.startsAt) : "Data não disponível"}
                    </p>
                    <div className="mt-3 flex gap-3 text-sm">
                      <span className="px-2 py-0.5 bg-surface rounded-md font-mono-feat text-xs uppercase">
                        {order.status === "confirmed" ? "Confirmado" : order.status}
                      </span>
                      <span className="text-muted-foreground">{formatBRL(order.totalAmount)}</span>
                    </div>
                  </div>
                </div>
                
                {/* Tickets list */}
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-2 p-5 bg-surface/50">
                  {order.items?.flatMap((item) => {
                    const ticketTypeName = events
                      .flatMap((e) => (e as any).ticketTypes || [])
                      .find((t) => t.id === item.ticketTypeId)?.name || firstTicketTypeName;

                    // If backend generated ticket codes, render them
                    const codes = (item as any).ticketCodes || [];
                    return codes.map((code: string, idx: number) => (
                      <div key={`${item.id}-${code}-${idx}`} className="bg-card rounded-lg p-3 border border-border flex justify-between items-center">
                        <div>
                          <p className="font-mono-feat text-[10px] uppercase text-muted-foreground">{ticketTypeName}</p>
                          <p className="font-mono-feat text-sm font-medium">{code}</p>
                        </div>
                        <span className="text-xs px-2 py-0.5 bg-primary text-primary-foreground rounded-md">Ativo</span>
                      </div>
                    ));
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
