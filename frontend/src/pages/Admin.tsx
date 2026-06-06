import { useState } from "react";
import { PageLayout } from "@/components/layout/PageLayout";
import { formatBRL, formatDateTime } from "@/data/mock";
import { Event, TicketType, Venue } from "@/types/domain";
import { eventsApi } from "@/lib/api";
import { useAuth } from "@/store/auth";
import { useQuery } from "@tanstack/react-query";
import { Calendar, MapPin, Plus, Ticket, Trash2 } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";

type Tab = "events" | "venues" | "tickets";

const Admin = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [tab, setTab] = useState<Tab>("events");

  // Enforce organizer authorization guard
  if (!user || user.role !== "admin") {
    return (
      <PageLayout>
        <div className="max-w-md mx-auto px-6 py-24 text-center">
          <h1 className="text-2xl font-semibold text-destructive">Acesso restrito</h1>
          <p className="text-muted-foreground mt-2">Você precisa estar autenticado como Organizador para acessar esta página.</p>
          <button
            onClick={() => navigate("/login?redirect=/admin")}
            className="mt-6 w-full bg-primary text-primary-foreground py-3 rounded-lg text-sm font-medium hover:opacity-90 transition-opacity"
          >
            Entrar como Organizador
          </button>
        </div>
      </PageLayout>
    );
  }

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
          {tab === "events" && <EventsPanel />}
          {tab === "venues" && <VenuesPanel />}
          {tab === "tickets" && <TicketTypesPanel />}
        </div>
      </div>
    </PageLayout>
  );
};

/* ---------- Events ---------- */
const EventsPanel = () => {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<Partial<Event>>({ status: "draft" });
  const [isSaving, setIsSaving] = useState(false);

  // Fetch all venues and events from real database
  const { data: events = [], refetch: refetchEvents, isLoading: eventsLoading } = useQuery({
    queryKey: ["admin-events"],
    queryFn: () => eventsApi.getAll(),
  });

  const { data: venues = [] } = useQuery({
    queryKey: ["admin-venues"],
    queryFn: () => eventsApi.getVenues(),
  });

  const save = async () => {
    if (!form.name || !form.startsAt || !form.venueId) {
      toast.error("Preencha nome, data de início e local");
      return;
    }
    
    setIsSaving(true);
    try {
      const startsAtIso = new Date(form.startsAt!).toISOString();
      const endsAtIso = new Date(form.endsAt || form.startsAt!).toISOString();
      
      await eventsApi.create(form.name!, startsAtIso, endsAtIso, form.venueId!);
      setForm({ status: "draft" });
      setOpen(false);
      refetchEvents();
      toast.success("Evento criado com sucesso!");
    } catch (err: any) {
      toast.error(err.message || "Erro ao criar o evento.");
    } finally {
      setIsSaving(false);
    }
  };

  const remove = async (id: string) => {
    if (!window.confirm("Deseja realmente excluir este evento?")) return;
    try {
      await eventsApi.delete(id);
      refetchEvents();
      toast.success("Evento excluído.");
    } catch (err: any) {
      toast.error(err.message || "Erro ao excluir o evento.");
    }
  };

  if (eventsLoading) {
    return <div className="text-center py-12 text-muted-foreground text-sm">Carregando eventos...</div>;
  }

  return (
    <>
      <Toolbar onAdd={() => setOpen(!open)} addLabel="Novo evento" />
      {open && (
        <div className="bg-card ring-1 ring-border rounded-2xl p-6 mb-6 grid grid-cols-1 md:grid-cols-2 gap-4">
          <Input label="Nome" value={form.name || ""} onChange={(v) => setForm({ ...form, name: v })} />
          <Input label="Categoria (Metadado Visual)" value={form.category || ""} onChange={(v) => setForm({ ...form, category: v })} />
          <Input label="Início" type="datetime-local" value={form.startsAt || ""} onChange={(v) => setForm({ ...form, startsAt: v })} />
          <Input label="Fim" type="datetime-local" value={form.endsAt || ""} onChange={(v) => setForm({ ...form, endsAt: v })} />
          
          <Select 
            label="Local" 
            value={form.venueId || ""} 
            onChange={(v) => setForm({ ...form, venueId: v })} 
            options={venues.map((v) => ({ value: v.id, label: v.name }))} 
          />
          
          <Select 
            label="Status" 
            value={form.status || "draft"} 
            onChange={(v) => setForm({ ...form, status: v as Event["status"] })} 
            options={[
              { value: "draft", label: "Rascunho" },
              { value: "published", label: "Publicado" },
              { value: "cancelled", label: "Cancelado" },
              { value: "finished", label: "Encerrado" },
            ]} 
          />
          
          <div className="md:col-span-2 flex justify-end gap-2">
            <button onClick={() => setOpen(false)} disabled={isSaving} className="px-4 py-2 text-sm border border-border rounded-lg">Cancelar</button>
            <button onClick={save} disabled={isSaving} className="px-4 py-2 text-sm bg-primary text-primary-foreground rounded-lg">{isSaving ? "Salvando..." : "Salvar"}</button>
          </div>
        </div>
      )}
      
      <Table headers={["Evento", "Local", "Início", "Status", ""]}>
        {events.map((e) => {
          const venue = venues.find((v) => v.id === e.venueId);
          return (
            <tr key={e.id} className="border-b border-border last:border-0">
              <td className="py-4 font-medium">{e.name}<div className="text-xs text-muted-foreground">{e.category}</div></td>
              <td className="py-4 text-sm text-muted-foreground"><MapPin className="w-3.5 h-3.5 inline mr-1" />{venue?.name || "Local não encontrado"}</td>
              <td className="py-4 text-sm text-muted-foreground"><Calendar className="w-3.5 h-3.5 inline mr-1" />{formatDateTime(e.startsAt)}</td>
              <td className="py-4"><Badge status={e.status} /></td>
              <td className="py-4 text-right"><IconBtn onClick={() => remove(e.id)} /></td>
            </tr>
          );
        })}
      </Table>
    </>
  );
};

/* ---------- Venues ---------- */
const VenuesPanel = () => {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<Partial<Venue>>({});
  const [isSaving, setIsSaving] = useState(false);

  // Fetch all venues from backend
  const { data: venues = [], refetch: refetchVenues, isLoading: venuesLoading } = useQuery({
    queryKey: ["admin-venues"],
    queryFn: () => eventsApi.getVenues(),
  });

  const save = async () => {
    if (!form.name || !form.address || !form.capacity) {
      toast.error("Preencha todos os campos");
      return;
    }
    
    setIsSaving(true);
    try {
      await eventsApi.createVenue(form.name!, form.address!, Number(form.capacity));
      setForm({});
      setOpen(false);
      refetchVenues();
      toast.success("Local criado com sucesso!");
    } catch (err: any) {
      toast.error(err.message || "Erro ao criar o local.");
    } finally {
      setIsSaving(false);
    }
  };

  const remove = async (id: string) => {
    if (!window.confirm("Deseja realmente excluir este local?")) return;
    try {
      await eventsApi.deleteVenue(id);
      refetchVenues();
      toast.success("Local excluído.");
    } catch (err: any) {
      toast.error(err.message || "Erro ao excluir o local.");
    }
  };

  if (venuesLoading) {
    return <div className="text-center py-12 text-muted-foreground text-sm">Carregando locais...</div>;
  }

  return (
    <>
      <Toolbar onAdd={() => setOpen(!open)} addLabel="Novo local" />
      {open && (
        <div className="bg-card ring-1 ring-border rounded-2xl p-6 mb-6 grid grid-cols-1 md:grid-cols-3 gap-4">
          <Input label="Nome" value={form.name || ""} onChange={(v) => setForm({ ...form, name: v })} />
          <Input label="Endereço" value={form.address || ""} onChange={(v) => setForm({ ...form, address: v })} />
          <Input label="Capacidade" type="number" value={String(form.capacity || "")} onChange={(v) => setForm({ ...form, capacity: Number(v) })} />
          <div className="md:col-span-3 flex justify-end gap-2">
            <button onClick={() => setOpen(false)} disabled={isSaving} className="px-4 py-2 text-sm border border-border rounded-lg">Cancelar</button>
            <button onClick={save} disabled={isSaving} className="px-4 py-2 text-sm bg-primary text-primary-foreground rounded-lg">{isSaving ? "Salvando..." : "Salvar"}</button>
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
const TicketTypesPanel = () => {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<Partial<TicketType>>({});
  const [isSaving, setIsSaving] = useState(false);

  // Fetch events and extract embedded ticket types
  const { data: events = [], refetch: refetchEvents, isLoading: eventsLoading } = useQuery({
    queryKey: ["admin-events"],
    queryFn: () => eventsApi.getAll(),
  });

  const ticketTypes = events.flatMap((e) => 
    ((e as any).ticketTypes || []).map((t: any) => ({
      ...t,
      event: e,
    }))
  );

  const save = async () => {
    if (!form.eventId || !form.name || !form.price || !form.totalQuantity) {
      toast.error("Preencha todos os campos");
      return;
    }
    
    setIsSaving(true);
    try {
      await eventsApi.createTicketType(
        form.eventId!,
        form.name!,
        Number(form.price),
        Number(form.totalQuantity)
      );
      setForm({});
      setOpen(false);
      refetchEvents();
      toast.success("Tipo de ingresso criado!");
    } catch (err: any) {
      toast.error(err.message || "Erro ao criar tipo de ingresso.");
    } finally {
      setIsSaving(false);
    }
  };

  const remove = () => {
    toast.error("Tipos de ingresso são vinculados ao evento e não podem ser excluídos individualmente.");
  };

  if (eventsLoading) {
    return <div className="text-center py-12 text-muted-foreground text-sm">Carregando tipos de ingresso...</div>;
  }

  return (
    <>
      <Toolbar onAdd={() => setOpen(!open)} addLabel="Novo tipo" />
      {open && (
        <div className="bg-card ring-1 ring-border rounded-2xl p-6 mb-6 grid grid-cols-1 md:grid-cols-2 gap-4">
          <Select 
            label="Evento" 
            value={form.eventId || ""} 
            onChange={(v) => setForm({ ...form, eventId: v })} 
            options={events.map((e) => ({ value: e.id, label: e.name }))} 
          />
          <Input label="Nome (ex: VIP)" value={form.name || ""} onChange={(v) => setForm({ ...form, name: v })} />
          <Input label="Preço (R$)" type="number" value={String(form.price || "")} onChange={(v) => setForm({ ...form, price: Number(v) })} />
          <Input label="Quantidade total" type="number" value={String(form.totalQuantity || "")} onChange={(v) => setForm({ ...form, totalQuantity: Number(v) })} />
          <div className="md:col-span-2 flex justify-end gap-2">
            <button onClick={() => setOpen(false)} disabled={isSaving} className="px-4 py-2 text-sm border border-border rounded-lg">Cancelar</button>
            <button onClick={save} disabled={isSaving} className="px-4 py-2 text-sm bg-primary text-primary-foreground rounded-lg">{isSaving ? "Salvando..." : "Salvar"}</button>
          </div>
        </div>
      )}
      <Table headers={["Tipo", "Evento", "Preço", "Disponível / Total", ""]}>
        {ticketTypes.map((t) => (
          <tr key={t.id} className="border-b border-border last:border-0">
            <td className="py-4 font-medium flex items-center gap-2"><Ticket className="w-4 h-4" />{t.name}</td>
            <td className="py-4 text-sm text-muted-foreground">{t.event?.name}</td>
            <td className="py-4 font-mono-feat text-sm">{formatBRL(t.price)}</td>
            <td className="py-4 font-mono-feat text-sm">{t.availableQuantity} / {t.totalQuantity}</td>
            <td className="py-4 text-right"><IconBtn onClick={remove} /></td>
          </tr>
        ))}
      </Table>
    </>
  );
};

/* ---------- shared ---------- */
const Toolbar = ({ onAdd, addLabel }: { onAdd: () => void; addLabel: string }) => (
  <div className="flex justify-end mb-4">
    <button onClick={onAdd} className="bg-primary text-primary-foreground text-sm font-medium px-4 py-2 rounded-lg flex items-center gap-2 hover:opacity-90 transition-opacity">
      <Plus className="w-4 h-4" /> {addLabel}
    </button>
  </div>
);

const Table = ({ headers, children }: { headers: string[]; children: React.ReactNode }) => (
  <div className="bg-card ring-1 ring-border rounded-2xl overflow-hidden shadow-sm">
    <table className="w-full">
      <thead>
        <tr className="border-b border-border bg-surface/50">
          {headers.map((h, i) => (
            <th key={i} className="text-left px-5 py-3 font-mono-feat text-[10px] uppercase tracking-wider text-muted-foreground">{h}</th>
          ))}
        </tr>
      </thead>
      <tbody className="px-5">
        {children}
      </tbody>
    </table>
    <style>{`tbody tr td:first-child{padding-left:1.25rem}tbody tr td:last-child{padding-right:1.25rem}`}</style>
  </div>
);

const Input = ({ label, value, onChange, type = "text" }: { label: string; value: string; onChange: (v: string) => void; type?: string }) => (
  <label className="block">
    <span className="font-mono-feat text-[10px] uppercase tracking-wider text-muted-foreground">{label}</span>
    <input type={type} value={value} onChange={(e) => onChange(e.target.value)}
      className="mt-1.5 w-full h-10 px-3 bg-background rounded-lg border border-border focus:border-foreground outline-none text-sm transition-colors" />
  </label>
);

const Select = ({ label, value, onChange, options }: { label: string; value: string; onChange: (v: string) => void; options: { value: string; label: string }[] }) => (
  <label className="block">
    <span className="font-mono-feat text-[10px] uppercase tracking-wider text-muted-foreground">{label}</span>
    <select value={value} onChange={(e) => onChange(e.target.value)}
      className="mt-1.5 w-full h-10 px-2 bg-background rounded-lg border border-border focus:border-foreground outline-none text-sm transition-colors">
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
