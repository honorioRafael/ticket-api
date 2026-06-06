import { Link, useParams } from "react-router-dom";
import { PageLayout } from "@/components/layout/PageLayout";
import { formatBRL, formatDateTime } from "@/data/mock";
import { salesApi } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import { CheckCircle2 } from "lucide-react";

const Success = () => {
  const { orderId = "" } = useParams();
  
  // Retrieve checkout metadata from session storage
  const meta = JSON.parse(sessionStorage.getItem(`order:${orderId}`) || "{}");
  const ticketCodes: string[] = meta.ticketCodes || [];

  // Fetch the order details from Sales API
  const { data: order, isLoading } = useQuery({
    queryKey: ["order", orderId],
    queryFn: () => salesApi.getOrderById(orderId),
    enabled: !!orderId,
  });

  if (isLoading) {
    return (
      <PageLayout>
        <div className="max-w-3xl mx-auto px-6 py-24 text-center">
          <h1 className="text-xl font-medium text-muted-foreground">Carregando confirmação...</h1>
        </div>
      </PageLayout>
    );
  }

  if (!order) {
    return (
      <PageLayout>
        <div className="max-w-3xl mx-auto px-6 py-24 text-center">
          <h1 className="text-2xl font-semibold">Pedido não encontrado</h1>
          <Link to="/" className="mt-6 inline-block bg-primary text-primary-foreground px-5 py-3 rounded-lg text-sm font-medium">
            Ver eventos
          </Link>
        </div>
      </PageLayout>
    );
  }

  return (
    <PageLayout>
      <div className="max-w-3xl mx-auto px-6 py-16">
        <div className="text-center">
          <div className="inline-grid place-items-center w-14 h-14 rounded-full bg-primary text-primary-foreground">
            <CheckCircle2 className="w-7 h-7" />
          </div>
          <h1 className="mt-6 text-3xl md:text-4xl font-semibold tracking-tight">Compra confirmada!</h1>
          <p className="mt-3 text-muted-foreground">
            Seus ingressos foram emitidos e enviados para <span className="font-medium text-foreground">{meta.email || "seu e-mail"}</span>.
          </p>
        </div>

        <div className="mt-10 bg-card ring-1 ring-border rounded-2xl p-6">
          <div className="flex justify-between text-sm">
            <span className="text-muted-foreground">Pedido</span>
            <span className="font-mono-feat">{order.id}</span>
          </div>
          <div className="mt-2 flex justify-between text-sm">
            <span className="text-muted-foreground">Data</span>
            <span>{formatDateTime(order.placedAt)}</span>
          </div>
          {meta.eventName && (
            <div className="mt-2 flex justify-between text-sm">
              <span className="text-muted-foreground">Evento</span>
              <span className="font-medium">{meta.eventName}</span>
            </div>
          )}
          <div className="mt-2 flex justify-between text-sm">
            <span className="text-muted-foreground">Total</span>
            <span className="font-semibold">{formatBRL(order.totalAmount)}</span>
          </div>
        </div>

        {ticketCodes.length > 0 && (
          <>
            <h2 className="mt-10 text-base font-semibold">Seus ingressos ({ticketCodes.length})</h2>
            <div className="mt-4 grid grid-cols-1 sm:grid-cols-2 gap-3">
              {ticketCodes.map((code) => (
                <div key={code} className="bg-surface rounded-xl p-4 border border-border">
                  <p className="font-mono-feat text-[10px] uppercase text-muted-foreground">Ingresso</p>
                  <p className="font-mono-feat text-lg font-medium mt-1">{code}</p>
                </div>
              ))}
            </div>
          </>
        )}

        <div className="mt-10 flex gap-3 justify-center">
          <Link to="/meus-ingressos" className="bg-primary text-primary-foreground text-sm font-medium px-5 py-3 rounded-lg">
            Ver meus ingressos
          </Link>
          <Link to="/" className="text-sm font-medium px-5 py-3 rounded-lg border border-border">
            Voltar à home
          </Link>
        </div>
      </div>
    </PageLayout>
  );
};

export default Success;
