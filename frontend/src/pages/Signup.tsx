import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { PageLayout } from "@/components/layout/PageLayout";
import { useAuth } from "@/store/auth";
import { toast } from "sonner";
import { Ticket } from "lucide-react";

const Signup = () => {
  const navigate = useNavigate();
  const signup = useAuth((s) => s.signup);
  const [name, setName] = useState("");
  const [cpf, setCpf] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name || !email || !cpf || password.length < 6) {
      toast.error("Preencha todos os campos (senha mín. 6 caracteres)");
      return;
    }
    
    setIsSubmitting(true);
    try {
      const u = await signup(name, email, password, cpf);
      if (!u) {
        toast.error("CPF ou E-mail inválidos / já cadastrados");
        return;
      }
      toast.success("Conta criada com sucesso! Você foi conectado.");
      navigate("/");
    } catch (err: any) {
      toast.error(err.message || "Erro ao criar conta");
    } finally {
      setIsSubmitting(false);
    }
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
        <p className="font-mono-feat text-xs uppercase tracking-widest text-muted-foreground">// Cadastro</p>
        <h1 className="mt-2 text-3xl font-semibold tracking-tight">Criar uma conta</h1>

        <form onSubmit={submit} className="mt-8 space-y-4">
          <Field label="Nome completo" value={name} onChange={setName} placeholder="Maria Silva" />
          <Field label="CPF" value={cpf} onChange={setCpf} placeholder="000.000.000-00" />
          <Field label="E-mail" type="email" value={email} onChange={setEmail} placeholder="voce@email.com" />
          <Field label="Senha" type="password" value={password} onChange={setPassword} placeholder="Mínimo 6 caracteres" />
          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full bg-primary text-primary-foreground text-sm font-medium py-3 rounded-lg hover:opacity-90 disabled:opacity-50 transition-opacity"
          >
            {isSubmitting ? "Criando conta..." : "Criar conta"}
          </button>
        </form>

        <p className="mt-6 text-sm text-muted-foreground text-center">
          Já tem conta?{" "}
          <Link to="/login" className="text-foreground font-medium hover:underline">
            Entrar
          </Link>
        </p>
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

export default Signup;
