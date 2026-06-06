import { useState } from "react";
import { PageLayout } from "@/components/layout/PageLayout";
import { events, ticketTypes, venues, formatBRL, formatDateTime, getVenue } from "@/data/mock";
import { Event, TicketType, Venue } from "@/types/domain";
import { Calendar, MapPin, Plus, Ticket, Trash2 } from "lucide-react";
import { toast } from "sonner";

type Tab = "events" | "venues" | "tickets";

const Admin = () => {
  const [tab, setTab] = useState<Tab>("events");
  const [, force] = useState(0);
  const refresh = () => force((n) => n + 1);

  return (
    <PageLayout>
      <div className="max-w-7xl mx-auto px-6 py-12">
        <p className="font-mono-feat text-xs uppercase tracking-widest text-muted-foreground">// Painel</p>
        <h1 className="mt-2 text-3xl md:text-4xl font-semibold tracking-tight">Administração</h1>

        <div className="mt-8 flex gap-1 border-b border-border">
          {(["events", "venues", "tickets"] as Tab[]).map((t) => (
            <button
              key={t}
              onClick={() => setTab(t)}
              className={`px-4 py-3 text-sm font-medium border-b-2 -mb-px transition-colors ${
                tab === t ? "border-foreground text-foreground" : "border-transparent text-muted-foreground hover:text-foreground"
              }`}
            >
              {t === "events" ? "Eventos" : t === "venues" ? "Locais" : "Tipos de ingresso"}
            </button>
          ))}
        </div>

        <div className="mt-8">
          {tab === "events" && <EventsPanel onChange={refresh} />}
          {tab === "venues" && <VenuesPanel onChange={refresh} />}
          {tab === "tickets" && <TicketTypesPanel onChange={refresh} />}
        </div>
      </div>
    </PageLayout>
  );
};

/* ---------- Events ---------- */
const EventsPanel = ({ onChange }: { onChange: () => void }) => {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<Partial<Event>>({ status: "draft" });

  const save = () => {
    if (!form.name || !form.startsAt || !form.venueId) {
      toast.error("Preencha nome, data e local");
      return;
    }
    events.push({
      id: "e" + Date.now(),
      name: form.name!,
      description: form.description,
      startsAt: new Date(form.startsAt!).toISOString(),
      endsAt: new Date(form.endsAt || form.startsAt!).toISOString(),
      status: (form.status as Event["status"]) || "draft",
      venueId: form.venueId!,
      category: form.category,
    });
    setForm({ status: "draft" });
    setOpen(false);
    onChange();
    toast.success("Evento criado");
  };

  const remove = (id: string) => {
    const i = events.findIndex((e) => e.id === id);
    if (i >= 0) events.splice(i, 1);
    onChange();
  };

  return (
    <>
      <Toolbar onAdd={() => setOpen(!open)} addLabel="Novo evento" />
      {open && (
        <div className="bg-card ring-1 ring-border rounded-2xl p-6 mb-6 grid grid-cols-1 md:grid-cols-2 gap-4">
          <Input label="Nome" value={form.name || ""} onChange={(v) => setForm({ ...form, name: v })} />
          <Input label="Categoria" value={form.category || ""} onChange={(v) => setForm({ ...form, category: v })} />
          <Input label="Início" type="datetime-local" value={form.startsAt || ""} onChange={(v) => setForm({ ...form, startsAt: v })} />
          <Input label="Fim" type="datetime-local" value={form.endsAt || ""} onChange={(v) => setForm({ ...form, endsAt: v })} />
          <Select label="Local" value={form.venueId || ""} onChange={(v) => setForm({ ...form, venueId: v })} options={venues.map((v) => ({ value: v.id, label: v.name }))} />
          <Select label="Status" value={form.status || "draft"} onChange={(v) => setForm({ ...form, status: v as Event["status"] })} options={[
            { value: "draft", label: "Rascunho" },
            { value: "published", label: "Publicado" },
            { value: "cancelled", label: "Cancelado" },
            { value: "finished", label: "Encerrado" },
          ]} />
          <div className="md:col-span-2 flex justify-end gap-2">
            <button onClick={() => setOpen(false)} className="px-4 py-2 text-sm border border-border rounded-lg">Cancelar</button>
            <button onClick={save} className="px-4 py-2 text-sm bg-primary text-primary-foreground rounded-lg">Salvar</button>
          </div>
        </div>
      )}
      <Table headers={["Evento", "Local", "Início", "Status", ""]}>
        {events.map((e) => (
          <tr key={e.id} className="border-b border-border last:border-0">
            <td className="py-4 font-medium">{e.name}<div className="text-xs text-muted-foreground">{e.category}</div></td>
            <td className="py-4 text-sm text-muted-foreground"><MapPin className="w-3.5 h-3.5 inline mr-1" />{getVenue(e.venueId)?.name}</td>
            <td className="py-4 text-sm text-muted-foreground"><Calendar className="w-3.5 h-3.5 inline mr-1" />{formatDateTime(e.startsAt)}</td>
            <td className="py-4"><Badge status={e.status} /></td>
            <td className="py-4 text-right"><IconBtn onClick={() => remove(e.id)} /></td>
          </tr>
        ))}
      </Table>
    </>
  );
};

/* ---------- Venues ---------- */
const VenuesPanel = ({ onChange }: { onChange: () => void }) => {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<Partial<Venue>>({});

  const save = () => {
    if (!form.name || !form.address || !form.capacity) {
      toast.error("Preencha todos os campos");
      return;
    }
    venues.push({ id: "v" + Date.now(), name: form.name!, address: form.address!, capacity: Number(form.capacity) });
    setForm({});
    setOpen(false);
    onChange();
    toast.success("Local criado");
  };

  const remove = (id: string) => {
    const i = venues.findIndex((v) => v.id === id);
    if (i >= 0) venues.splice(i, 1);
    onChange();
  };

  return (
    <>
      <Toolbar onAdd={() => setOpen(!open)} addLabel="Novo local" />
      {open && (
        <div className="bg-card ring-1 ring-border rounded-2xl p-6 mb-6 grid grid-cols-1 md:grid-cols-3 gap-4">
          <Input label="Nome" value={form.name || ""} onChange={(v) => setForm({ ...form, name: v })} />
          <Input label="Endereço" value={form.address || ""} onChange={(v) => setForm({ ...form, address: v })} />
          <Input label="Capacidade" type="number" value={String(form.capacity || "")} onChange={(v) => setForm({ ...form, capacity: Number(v) })} />
          <div className="md:col-span-3 flex justify-end gap-2">
            <button onClick={() => setOpen(false)} className="px-4 py-2 text-sm border border-border rounded-lg">Cancelar</button>
            <button onClick={save} className="px-4 py-2 text-sm bg-primary text-primary-foreground rounded-lg">Salvar</button>
          </div>
        </div>
      )}
      <Table headers={["Local", "Endereço", "Capacidade", ""]}>
        {venues.map((v) => (
          <tr key={v.id} className="border-b border-border last:border-0">
            <td className="py-4 font-medium">{v.name}</td>
            <td className="py-4 text-sm text-muted-foreground">{v.address}</td>
            <td className="py-4 font-mono-feat text-sm">{v.capacity}</td>
            <td className="py-4 text-right"><IconBtn onClick={() => remove(v.id)} /></td>
          </tr>
        ))}
      </Table>
    </>
  );
};

/* ---------- TicketTypes ---------- */
const TicketTypesPanel = ({ onChange }: { onChange: () => void }) => {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<Partial<TicketType>>({});

  const save = () => {
    if (!form.eventId || !form.name || !form.price || !form.totalQuantity) {
      toast.error("Preencha todos os campos");
      return;
    }
    ticketTypes.push({
      id: "t" + Date.now(),
      eventId: form.eventId!,
      name: form.name!,
      price: Number(form.price),
      totalQuantity: Number(form.totalQuantity),
      availableQuantity: Number(form.totalQuantity),
    });
    setForm({});
    setOpen(false);
    onChange();
    toast.success("Tipo de ingresso criado");
  };

  const remove = (id: string) => {
    const i = ticketTypes.findIndex((t) => t.id === id);
    if (i >= 0) ticketTypes.splice(i, 1);
    onChange();
  };

  return (
    <>
      <Toolbar onAdd={() => setOpen(!open)} addLabel="Novo tipo" />
      {open && (
        <div className="bg-card ring-1 ring-border rounded-2xl p-6 mb-6 grid grid-cols-1 md:grid-cols-2 gap-4">
          <Select label="Evento" value={form.eventId || ""} onChange={(v) => setForm({ ...form, eventId: v })} options={events.map((e) => ({ value: e.id, label: e.name }))} />
          <Input label="Nome (ex: VIP)" value={form.name || ""} onChange={(v) => setForm({ ...form, name: v })} />
          <Input label="Preço (R$)" type="number" value={String(form.price || "")} onChange={(v) => setForm({ ...form, price: Number(v) })} />
          <Input label="Quantidade total" type="number" value={String(form.totalQuantity || "")} onChange={(v) => setForm({ ...form, totalQuantity: Number(v) })} />
          <div className="md:col-span-2 flex justify-end gap-2">
            <button onClick={() => setOpen(false)} className="px-4 py-2 text-sm border border-border rounded-lg">Cancelar</button>
            <button onClick={save} className="px-4 py-2 text-sm bg-primary text-primary-foreground rounded-lg">Salvar</button>
          </div>
        </div>
      )}
      <Table headers={["Tipo", "Evento", "Preço", "Disponível / Total", ""]}>
        {ticketTypes.map((t) => {
          const ev = events.find((e) => e.id === t.eventId);
          return (
            <tr key={t.id} className="border-b border-border last:border-0">
              <td className="py-4 font-medium flex items-center gap-2"><Ticket className="w-4 h-4" />{t.name}</td>
              <td className="py-4 text-sm text-muted-foreground">{ev?.name}</td>
              <td className="py-4 font-mono-feat text-sm">{formatBRL(t.price)}</td>
              <td className="py-4 font-mono-feat text-sm">{t.availableQuantity} / {t.totalQuantity}</td>
              <td className="py-4 text-right"><IconBtn onClick={() => remove(t.id)} /></td>
            </tr>
          );
        })}
      </Table>
    </>
  );
};

/* ---------- shared ---------- */
const Toolbar = ({ onAdd, addLabel }: { onAdd: () => void; addLabel: string }) => (
  <div className="flex justify-end mb-4">
    <button onClick={onAdd} className="bg-primary text-primary-foreground text-sm font-medium px-4 py-2 rounded-lg flex items-center gap-2">
      <Plus className="w-4 h-4" /> {addLabel}
    </button>
  </div>
);

const Table = ({ headers, children }: { headers: string[]; children: React.ReactNode }) => (
  <div className="bg-card ring-1 ring-border rounded-2xl overflow-hidden">
    <table className="w-full">
      <thead>
        <tr className="border-b border-border bg-surface/50">
          {headers.map((h, i) => (
            <th key={i} className="text-left px-5 py-3 font-mono-feat text-[10px] uppercase tracking-wider text-muted-foreground">{h}</th>
          ))}
        </tr>
      </thead>
      <tbody className="px-5">
        {/* spacing via td padding */}
        {Array.isArray(children) ? children : children}
      </tbody>
    </table>
    <style>{`tbody tr td:first-child{padding-left:1.25rem}tbody tr td:last-child{padding-right:1.25rem}`}</style>
  </div>
);

const Input = ({ label, value, onChange, type = "text" }: { label: string; value: string; onChange: (v: string) => void; type?: string }) => (
  <label className="block">
    <span className="font-mono-feat text-[10px] uppercase tracking-wider text-muted-foreground">{label}</span>
    <input type={type} value={value} onChange={(e) => onChange(e.target.value)}
      className="mt-1.5 w-full h-10 px-3 bg-background rounded-lg border border-border focus:border-foreground outline-none text-sm" />
  </label>
);

const Select = ({ label, value, onChange, options }: { label: string; value: string; onChange: (v: string) => void; options: { value: string; label: string }[] }) => (
  <label className="block">
    <span className="font-mono-feat text-[10px] uppercase tracking-wider text-muted-foreground">{label}</span>
    <select value={value} onChange={(e) => onChange(e.target.value)}
      className="mt-1.5 w-full h-10 px-2 bg-background rounded-lg border border-border focus:border-foreground outline-none text-sm">
      <option value="">Selecione...</option>
      {options.map((o) => <option key={o.value} value={o.value}>{o.label}</option>)}
    </select>
  </label>
);

const Badge = ({ status }: { status: string }) => {
  const map: Record<string, string> = {
    published: "bg-primary text-primary-foreground",
    draft: "bg-surface text-muted-foreground",
    cancelled: "bg-destructive/10 text-destructive",
    finished: "bg-surface text-muted-foreground",
  };
  return <span className={`text-xs font-medium px-2 py-1 rounded-md ${map[status] || ""}`}>{status}</span>;
};

const IconBtn = ({ onClick }: { onClick: () => void }) => (
  <button onClick={onClick} className="p-2 rounded-lg text-muted-foreground hover:text-destructive hover:bg-destructive/10 transition-colors">
    <Trash2 className="w-4 h-4" />
  </button>
);

export default Admin;
