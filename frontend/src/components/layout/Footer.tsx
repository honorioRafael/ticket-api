export const Footer = () => (
  <footer className="border-t border-border mt-24 py-12">
    <div className="max-w-7xl mx-auto px-6 flex flex-col md:flex-row justify-between gap-8">
      <div className="max-w-xs">
        <p className="text-lg font-semibold tracking-tight">Tikket</p>
        <p className="mt-3 text-sm text-muted-foreground">
          Plataforma de descoberta de eventos e gestão de ingressos.
        </p>
      </div>
      <div className="flex gap-12 font-mono-feat text-xs uppercase text-muted-foreground">
        <span>© 2026 Tikket</span>
        <span>Termos</span>
        <span>Privacidade</span>
      </div>
    </div>
  </footer>
);
