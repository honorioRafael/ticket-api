import { create } from "zustand";
import { persist } from "zustand/middleware";
import { authApi, registerTokenProvider } from "@/lib/api";

export type UserRole = "admin" | "user";

export interface AuthUser {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  token: string;
}

interface AuthState {
  user: AuthUser | null;
  login: (email: string, password: string) => Promise<AuthUser | null>;
  signup: (name: string, email: string, password: string, cpf: string) => Promise<AuthUser | null>;
  logout: () => void;
}

export const useAuth = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      login: async (email, password) => {
        try {
          // Attempt user (customer) login
          const res = await authApi.loginCustomer(email, password);
          const auth: AuthUser = {
            id: res.customer.id,
            name: res.customer.name,
            email: res.customer.email,
            role: "user",
            token: res.token,
          };
          set({ user: auth });
          return auth;
        } catch {
          // If customer login fails, attempt organizer (admin) login
          try {
            const res = await authApi.loginOrganizer(email, password);
            const auth: AuthUser = {
              id: res.organizer?.id || "admin-id",
              name: res.organizer?.name || "Organizador",
              email: email,
              role: "admin",
              token: res.token,
            };
            set({ user: auth });
            return auth;
          } catch {
            return null;
          }
        }
      },
      signup: async (name, email, password, cpf) => {
        try {
          const res = await authApi.registerCustomer(name, email, cpf, password);
          const auth: AuthUser = {
            id: res.customer.id,
            name: res.customer.name,
            email: res.customer.email,
            role: "user",
            token: res.token,
          };
          set({ user: auth });
          return auth;
        } catch {
          return null;
        }
      },
      logout: () => set({ user: null }),
    }),
    { name: "tikket-auth" }
  )
);

registerTokenProvider(() => useAuth.getState().user?.token || null);
