import { useNavigate } from "react-router-dom";
import { PageLayout } from "@/components/layout/PageLayout";
import { formatBRL, getEvent, getTicketType } from "@/data/mock";
import { useCart } from "@/store/cart";
import { useState } from "react";
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
  const { eventId, lines, total, checkout, clear } = useCart();
  const event = eventId ? getEvent(eventId) : undefined;
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [doc, setDoc] = useState("");
  const [method, setMethod] = useState<PaymentMethod>("credit_card");

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

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!name || !email || !doc) {
      toast.error("Preencha todos os dados");
      return;
    }
    const customerId = "c-" + Date.now();
    const { order } = checkout(customerId, method);
    // store customer info on the order id for confirmation page
    sessionStorage.setItem(
      `order:${order.id}`,
      JSON.stringify({ name, email, eventId: event.id })
    );
    clear();
    navigate(`/sucesso/${order.id}`);
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
                  const t = getTicketType(l.ticketTypeId)!;
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
                className="mt-6 w-full bg-primary text-primary-foreground text-sm font-medium py-3 rounded-lg hover:opacity-90 transition-opacity"
              >
                Confirmar e pagar
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
