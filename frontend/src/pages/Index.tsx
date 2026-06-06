import { useState } from "react";
import { EventCard } from "@/components/EventCard";
import { PageLayout } from "@/components/layout/PageLayout";
import { events } from "@/data/mock";
import { Search } from "lucide-react";
import heroImg from "@/assets/hero.jpg";

const categories = ["Todos", "Música", "Festival", "Jazz", "Comédia"];

const Index = () => {
  const [query, setQuery] = useState("");
  const [cat, setCat] = useState("Todos");

  const filtered = events.filter((e) => {
    const matchCat = cat === "Todos" || e.category === cat;
    const matchQuery = !query || e.name.toLowerCase().includes(query.toLowerCase());
    return matchCat && matchQuery;
  });

  return (
    <PageLayout>
      {/* Hero compacto */}
      <header className="relative overflow-hidden border-b border-border">
        <div className="absolute inset-0 -z-10">
          <img src={heroImg} alt="" className="w-full h-full object-cover opacity-[0.06]" />
          <div className="absolute inset-0 bg-gradient-to-b from-background/40 via-background/80 to-background" />
        </div>
        <div className="max-w-7xl mx-auto px-6 py-10 md:py-14">
          <p className="font-mono-feat text-[10px] uppercase tracking-widest text-muted-foreground mb-2">
            // Tikket
          </p>
          <h1 className="text-2xl md:text-3xl font-semibold tracking-tight text-balance max-w-2xl">
            Encontre sua próxima experiência cultural.
          </h1>

          <div className="mt-6 max-w-2xl relative">
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <input
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              type="text"
              placeholder="Artistas, clubes, cidades..."
              className="w-full h-12 pl-11 pr-4 bg-card rounded-xl ring-1 ring-border focus:ring-foreground outline-none transition-all text-sm placeholder:text-muted-foreground shadow-sm"
            />
          </div>
        </div>
      </header>

      {/* Events */}
      <section className="py-10">
        <div className="max-w-7xl mx-auto px-6">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
            <div>
              <h2 className="text-xl font-semibold">Próximos eventos</h2>
              <p className="text-sm text-muted-foreground mt-1">{filtered.length} eventos disponíveis</p>
            </div>
            <div className="flex gap-1 flex-wrap">
              {categories.map((c) => (
                <button
                  key={c}
                  onClick={() => setCat(c)}
                  className={`text-xs font-medium px-3 py-1.5 rounded-full transition-colors ${
                    cat === c
                      ? "bg-primary text-primary-foreground"
                      : "bg-surface text-muted-foreground hover:text-foreground"
                  }`}
                >
                  {c}
                </button>
              ))}
            </div>
          </div>

          {filtered.length > 0 ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {filtered.map((e) => (
                <EventCard key={e.id} event={e} />
              ))}
            </div>
          ) : (
            <div className="text-center py-20 text-muted-foreground">Nenhum evento encontrado.</div>
          )}
        </div>
      </section>
    </PageLayout>
  );
};

export default Index;
