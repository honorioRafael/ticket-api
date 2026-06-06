import { useNavigate, useParams, Link } from "react-router-dom";
import { PageLayout } from "@/components/layout/PageLayout";
import { formatBRL } from "@/data/mock";
import { useCart } from "@/store/cart";
import { eventsApi } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import { ArrowLeft, Minus, Plus } from "lucide-react";
import { useEffect } from "react";

const EventTickets = () => {
  const { id = "" } = useParams();
  const navigate = useNavigate();

  const { data: event, isLoading } = useQuery({
    queryKey: ["event", id],
    queryFn: () => eventsApi.getById(id),
    enabled: !!id,
  });

  const types = event?.ticketTypes || [];
  const { lines, setEvent, setQty, total } = useCart();

  useEffect(() => {
    if (event) setEvent(event.id);
  }, [event, setEvent]);

  if (isLoading) {
    return (
      <PageLayout>
        <div className="max-w-7xl mx-auto px-6 py-24 text-center">
          <h1 className="text-2xl font-semibold">Carregando ingressos do evento...</h1>
        </div>
      </PageLayout>
    );
  }

  if (!event) {
    return (
      <PageLayout>
        <div className="max-w-7xl mx-auto px-6 py-24 text-center">
          <h1 className="text-2xl font-semibold">Evento não encontrado</h1>
        </div>
      </PageLayout>
    );
  }

  const qtyOf = (tid: string) => lines.find((l) => l.ticketTypeId === tid)?.quantity ?? 0;
  const totalQty = lines.reduce((s, l) => s + l.quantity, 0);

  return (
    <PageLayout>
      <div className="max-w-3xl mx-auto px-6 py-12">
        <Link to={`/evento/${event.id}`} className="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors">
          <ArrowLeft className="w-4 h-4" /> Voltar ao evento
        </Link>

        <p className="mt-6 font-mono-feat text-xs uppercase tracking-widest text-muted-foreground">// Ingressos</p>
        <h1 className="mt-2 text-3xl md:text-4xl font-semibold tracking-tight">{event.name}</h1>
        <p className="mt-2 text-sm text-muted-foreground">Escolha o tipo e a quantidade de ingressos.</p>

        <div className="mt-10 bg-card ring-1 ring-border rounded-2xl p-6 shadow-md">
          <div className="space-y-3">
            {types.map((t) => {
              const q = qtyOf(t.id);
              const sold = t.availableQuantity === 0;
              return (
                <div key={t.id} className="rounded-xl border border-border p-4">
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <p className="font-medium">{t.name}</p>
                      <p className="text-sm text-muted-foreground mt-0.5">{formatBRL(t.price)}</p>
                      <p className="font-mono-feat text-[10px] uppercase text-muted-foreground mt-1">
                        {sold ? "Esgotado" : `${t.availableQuantity} disponíveis`}
                      </p>
                    </div>
                    <div className="flex items-center gap-2">
                      <button
                        onClick={() => setQty(t.id, Math.max(0, q - 1), t)}
                        disabled={q === 0}
                        className="w-8 h-8 grid place-items-center rounded-lg border border-border disabled:opacity-30 hover:bg-surface transition-colors"
                      >
                        <Minus className="w-3.5 h-3.5" />
                      </button>
                      <span className="w-6 text-center font-mono-feat text-sm">{q}</span>
                      <button
                        onClick={() => setQty(t.id, Math.min(t.availableQuantity, q + 1), t)}
                        disabled={sold || q >= t.availableQuantity}
                        className="w-8 h-8 grid place-items-center rounded-lg border border-border disabled:opacity-30 hover:bg-surface transition-colors"
                      >
                        <Plus className="w-3.5 h-3.5" />
                      </button>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>

          <div className="mt-6 pt-5 border-t border-border flex items-center justify-between">
            <div>
              <p className="font-mono-feat text-[10px] uppercase text-muted-foreground">Total ({totalQty})</p>
              <p className="text-xl font-semibold">{formatBRL(total())}</p>
            </div>
            <button
              onClick={() => navigate("/checkout")}
              disabled={totalQty === 0}
              className="bg-primary text-primary-foreground text-sm font-medium px-5 py-3 rounded-lg disabled:opacity-30 disabled:cursor-not-allowed hover:opacity-90 transition-opacity"
            >
              Continuar para pagamento
            </button>
          </div>
        </div>
      </div>
    </PageLayout>
  );
};

export default EventTickets;
