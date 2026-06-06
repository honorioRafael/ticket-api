import { Link, NavLink, useNavigate } from "react-router-dom";
import { LogOut, ShoppingCart, Ticket } from "lucide-react";
import { useAuth } from "@/store/auth";
import { useCart } from "@/store/cart";

export const Navbar = () => {
  const navigate = useNavigate();
  const user = useAuth((s) => s.user);
  const logout = useAuth((s) => s.logout);
  const lines = useCart((s) => s.lines);
  const eventId = useCart((s) => s.eventId);
  const cartCount = lines.reduce((s, l) => s + l.quantity, 0);

  const link = ({ isActive }: { isActive: boolean }) =>
    `text-sm font-medium transition-colors ${
      isActive ? "text-foreground" : "text-muted-foreground hover:text-foreground"
    }`;

  const isAdmin = user?.role === "admin";

  return (
    <nav className="sticky top-0 z-50 bg-background/80 backdrop-blur-md border-b border-border">
      <div className="max-w-7xl mx-auto px-6 h-16 flex items-center justify-between">
        <div className="flex items-center gap-10">
          <Link to="/" className="flex items-center gap-2">
            <div className="w-7 h-7 rounded-lg bg-primary text-primary-foreground grid place-items-center">
              <Ticket className="w-4 h-4" />
            </div>
            <span className="text-lg font-semibold tracking-tight">Tikket</span>
          </Link>
          <div className="hidden md:flex items-center gap-6">
            <NavLink to="/" end className={link}>Eventos</NavLink>
            {user && <NavLink to="/meus-ingressos" className={link}>Meus Ingressos</NavLink>}
            {isAdmin && <NavLink to="/admin" className={link}>Admin</NavLink>}
          </div>
        </div>
        <div className="flex items-center gap-2">
          <button
            onClick={() => navigate(eventId ? `/evento/${eventId}/ingressos` : "/")}
            className="relative w-10 h-10 grid place-items-center rounded-lg hover:bg-surface transition-colors"
            aria-label="Carrinho"
          >
            <ShoppingCart className="w-5 h-5" />
            {cartCount > 0 && (
              <span className="absolute -top-0.5 -right-0.5 min-w-[18px] h-[18px] px-1 rounded-full bg-primary text-primary-foreground text-[10px] font-mono-feat font-semibold grid place-items-center">
                {cartCount}
              </span>
            )}
          </button>

          {user ? (
            <>
              <span className="hidden sm:inline-block text-sm text-muted-foreground">
                Olá, <span className="text-foreground font-medium">{user.name.split(" ")[0]}</span>
              </span>
              <button
                onClick={() => { logout(); navigate("/"); }}
                className="w-10 h-10 grid place-items-center rounded-lg hover:bg-surface transition-colors"
                aria-label="Sair"
              >
                <LogOut className="w-4 h-4" />
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors hidden sm:inline-block px-2">
                Entrar
              </Link>
              <Link to="/criar-conta" className="bg-primary text-primary-foreground text-sm font-medium py-2 px-3 rounded-lg hover:opacity-90 transition-opacity">
                Criar conta
              </Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
};
