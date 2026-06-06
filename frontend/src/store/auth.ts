import { create } from "zustand";
import { persist } from "zustand/middleware";

export type UserRole = "admin" | "user";

export interface AuthUser {
  id: string;
  name: string;
  email: string;
  role: UserRole;
}

interface AuthState {
  user: AuthUser | null;
  users: (AuthUser & { password: string })[];
  login: (email: string, password: string) => AuthUser | null;
  signup: (name: string, email: string, password: string) => AuthUser | null;
  logout: () => void;
}

const seedUsers = [
  { id: "u-admin", name: "Administrador", email: "admin@tikket.com", password: "admin123", role: "admin" as UserRole },
  { id: "u-demo", name: "Maria Silva", email: "user@tikket.com", password: "user123", role: "user" as UserRole },
];

export const useAuth = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      users: seedUsers,
      login: (email, password) => {
        const u = get().users.find((x) => x.email.toLowerCase() === email.toLowerCase() && x.password === password);
        if (!u) return null;
        const auth: AuthUser = { id: u.id, name: u.name, email: u.email, role: u.role };
        set({ user: auth });
        return auth;
      },
      signup: (name, email, password) => {
        if (get().users.some((u) => u.email.toLowerCase() === email.toLowerCase())) return null;
        const newUser = { id: "u-" + Date.now(), name, email, password, role: "user" as UserRole };
        set((s) => ({ users: [...s.users, newUser], user: { id: newUser.id, name, email, role: "user" } }));
        return { id: newUser.id, name, email, role: "user" };
      },
      logout: () => set({ user: null }),
    }),
    { name: "tikket-auth" }
  )
);
