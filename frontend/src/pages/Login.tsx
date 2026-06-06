import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { PageLayout } from "@/components/layout/PageLayout";
import { useAuth } from "@/store/auth";
import { toast } from "sonner";
import { Ticket } from "lucide-react";

const Login = () => {
  const navigate = useNavigate();
  const login = useAuth((s) => s.login);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const submit = (e: React.FormEvent) => {
    e.preventDefault();
    const u = login(email, password);
    if (!u) {
      toast.error("E-mail ou senha inválidos");
      return;
    }
    toast.success(`Bem-vindo(a), ${u.name}`);
    navigate(u.role === "admin" ? "/admin" : "/");
  };

  return (
    <PageLayout>
      <div className="max-w-md mx-auto px-6 py-20">
        <div className="flex items-center gap-2 mb-8">
          <div className="w-7 h-7 rounded-lg bg-primary text-primary-foreground grid place-items-center">
            <Ticket className="w-4 h-4" />
          </div>
          <span className="text-lg font-semibold tracking-tight">Tikket</span>
        </div>
        <p className="font-mono-feat text-xs uppercase tracking-widest text-muted-foreground">// Acesso</p>
        <h1 className="mt-2 text-3xl font-semibold tracking-tight">Entrar na sua conta</h1>

        <form onSubmit={submit} className="mt-8 space-y-4">
          <Field label="E-mail" type="email" value={email} onChange={setEmail} placeholder="voce@email.com" />
          <Field label="Senha" type="password" value={password} onChange={setPassword} placeholder="••••••••" />
          <button
            type="submit"
            className="w-full bg-primary text-primary-foreground text-sm font-medium py-3 rounded-lg hover:opacity-90 transition-opacity"
          >
            Entrar
          </button>
        </form>

        <p className="mt-6 text-sm text-muted-foreground text-center">
          Não tem uma conta?{" "}
          <Link to="/criar-conta" className="text-foreground font-medium hover:underline">
            Criar conta
          </Link>
        </p>

        <div className="mt-8 p-4 rounded-xl bg-surface text-xs text-muted-foreground space-y-1">
          <p className="font-mono-feat uppercase tracking-wider">Contas de demonstração</p>
          <p>admin@tikket.com / admin123</p>
          <p>user@tikket.com / user123</p>
        </div>
      </div>
    </PageLayout>
  );
};

const Field = ({
  label, value, onChange, type = "text", placeholder,
}: { label: string; value: string; onChange: (v: string) => void; type?: string; placeholder?: string }) => (
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

export default Login;
