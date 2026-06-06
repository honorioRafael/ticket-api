import { useNavigate } from "react-router-dom";
import { PageLayout } from "@/components/layout/PageLayout";
import { formatBRL } from "@/data/mock";
import { useCart } from "@/store/cart";
import { useAuth } from "@/store/auth";
import { eventsApi } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import { PaymentMethod } from "@/types/domain";
import { CreditCard, QrCode, FileText } from "lucide-react";
import { toast } from "sonner";

const methods: { id: PaymentMethod; label: string; icon: React.ReactNode; hint: string }[] = [
  { id: "credit_card", label: "Cartão de crédito", icon: <CreditCard className="w-4 h-4" />, hint: "Aprovação imediata" },
  { id: "pix", label: "Pix", icon: <QrCode className="w-4 h-4" />, hint: "QR code instantâneo" },
  { id: "boleto", label: "Boleto", icon: <FileText className="w-4 h-4" />, hint: "Compensação em até 3 dias" },
];

const Checkout = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { eventId, lines, total, checkout, ticketTypes } = useCart();
  
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [doc, setDoc] = useState("");
  const [method, setMethod] = useState<PaymentMethod>("credit_card");
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Fetch event details dynamically from database
  const { data: event, isLoading: eventLoading } = useQuery({
    queryKey: ["event", eventId],
    queryFn: () => eventsApi.getById(eventId || ""),
    enabled: !!eventId,
  });

  // Pre-fill user details if logged in
  useEffect(() => {
    if (user) {
      setName(user.name || "");
      setEmail(user.email || "");
    }
  }, [user]);

  if (!user) {
    return (
      <PageLayout>
        <div className="max-w-md mx-auto px-6 py-24 text-center">
          <h1 className="text-2xl font-semibold">Identificação necessária</h1>
          <p className="text-muted-foreground mt-2">Você precisa entrar na sua conta para finalizar a compra de ingressos.</p>
          <button
            onClick={() => navigate("/login?redirect=/checkout")}
            className="mt-6 w-full bg-primary text-primary-foreground py-3 rounded-lg text-sm font-medium hover:opacity-90 transition-opacity"
          >
            Entrar ou Criar conta
          </button>
        </div>
      </PageLayout>
    );
  }

  if (eventLoading) {
    return (
      <PageLayout>
        <div className="max-w-3xl mx-auto px-6 py-24 text-center">
          <h1 className="text-xl font-medium text-muted-foreground">Carregando dados do pedido...</h1>
        </div>
      </PageLayout>
    );
  }

  if (!event || lines.length === 0) {
    return (
      <PageLayout>
        <div className="max-w-3xl mx-auto px-6 py-24 text-center">
          <h1 className="text-2xl font-semibold">Seu carrinho está vazio</h1>
          <button
            onClick={() => navigate("/")}
            className="mt-6 bg-primary text-primary-foreground px-5 py-3 rounded-lg text-sm font-medium"
          >
            Ver eventos
          </button>
        </div>
      </PageLayout>
    );
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name || !email || !doc) {
      toast.error("Preencha todos os dados");
      return;
    }
    
    setIsSubmitting(true);
    try {
      const { order, ticketCodes } = await checkout(method);
      // Store checkout metadata in sessionStorage for the Success confirmation page
      sessionStorage.setItem(
        `order:${order.id}`,
        JSON.stringify({ 
          name, 
          email, 
          eventName: event.name, 
          ticketCodes 
        })
      );
      toast.success("Pagamento aprovado!");
      navigate(`/sucesso/${order.id}`);
    } catch (err: any) {
      toast.error(err.message || "Não foi possível finalizar o pedido.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <PageLayout>
      <div className="max-w-5xl mx-auto px-6 py-12">
        <p className="font-mono-feat text-xs uppercase tracking-widest text-muted-foreground">// Checkout</p>
        <h1 className="mt-2 text-3xl md:text-4xl font-semibold tracking-tight">Finalize sua compra</h1>

        <form onSubmit={handleSubmit} className="mt-10 grid grid-cols-1 lg:grid-cols-5 gap-8">
          <div className="lg:col-span-3 space-y-8">
            <Section title="Dados do comprador">
              <Field label="Nome completo" value={name} onChange={setName} placeholder="Maria Silva" />
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <Field label="E-mail" value={email} onChange={setEmail} type="email" placeholder="maria@email.com" />
                <Field label="CPF / CNPJ" value={doc} onChange={setDoc} placeholder="000.000.000-00" />
              </div>
            </Section>

            <Section title="Forma de pagamento">
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                {methods.map((m) => (
                  <button
                    type="button"
                    key={m.id}
                    onClick={() => setMethod(m.id)}
                    className={`text-left p-4 rounded-xl border transition-all ${
                      method === m.id
                        ? "border-foreground bg-surface"
                        : "border-border hover:border-foreground/30"
                    }`}
                  >
                    <div className="flex items-center gap-2 mb-2">{m.icon}<span className="text-sm font-medium">{m.label}</span></div>
                    <p className="text-xs text-muted-foreground">{m.hint}</p>
                  </button>
                ))}
              </div>
            </Section>
          </div>

          {/* Summary */}
          <aside className="lg:col-span-2">
            <div className="bg-card ring-1 ring-border rounded-2xl p-6 shadow-md sticky top-24">
              <h2 className="text-base font-semibold">Resumo</h2>
              <p className="text-sm text-muted-foreground mt-1">{event.name}</p>
              <div className="mt-5 space-y-3">
                {lines.map((l) => {
                  const t = ticketTypes.find((x) => x.id === l.ticketTypeId)!;
                  if (!t) return null;
                  return (
                    <div key={l.ticketTypeId} className="flex justify-between text-sm">
                      <span className="text-muted-foreground">
                        {l.quantity}× {t.name}
                      </span>
                      <span className="font-medium">{formatBRL(t.price * l.quantity)}</span>
                    </div>
                  );
                })}
              </div>
              <div className="mt-5 pt-5 border-t border-border flex justify-between items-center">
                <span className="font-mono-feat text-[10px] uppercase text-muted-foreground">Total</span>
                <span className="text-xl font-semibold">{formatBRL(total())}</span>
              </div>
              <button
                type="submit"
                disabled={isSubmitting}
                className="mt-6 w-full bg-primary text-primary-foreground text-sm font-medium py-3 rounded-lg hover:opacity-90 disabled:opacity-50 transition-opacity"
              >
                {isSubmitting ? "Processando..." : "Confirmar e pagar"}
              </button>
            </div>
          </aside>
        </form>
      </div>
    </PageLayout>
  );
};

const Section = ({ title, children }: { title: string; children: React.ReactNode }) => (
  <div className="bg-card ring-1 ring-border rounded-2xl p-6">
    <h2 className="text-base font-semibold mb-5">{title}</h2>
    <div className="space-y-4">{children}</div>
  </div>
);

const Field = ({
  label,
  value,
  onChange,
  type = "text",
  placeholder,
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
  type?: string;
  placeholder?: string;
}) => (
  <label className="block">
    <span className="font-mono-feat text-[10px] uppercase tracking-wider text-muted-foreground">{label}</span>
    <input
      type={type}
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder}
      className="mt-1.5 w-full h-11 px-3 bg-background rounded-lg border border-border focus:border-foreground outline-none transition-colors text-sm"
    />
  </label>
);

export default Checkout;
