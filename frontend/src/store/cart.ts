import { create } from "zustand";
import { Order, TicketType } from "@/types/domain";
import { salesApi } from "@/lib/api";

interface CartLine {
  ticketTypeId: string;
  quantity: number;
}

interface CartState {
  eventId: string | null;
  lines: CartLine[];
  ticketTypes: TicketType[];
  setEvent: (eventId: string) => void;
  setQty: (ticketTypeId: string, quantity: number, ticketType?: TicketType) => void;
  clear: () => void;
  total: () => number;
  checkout: (method: "credit_card" | "pix" | "boleto") => Promise<{ order: Order; ticketCodes: string[] }>;
}

export const useCart = create<CartState>((set, get) => ({
  eventId: null,
  lines: [],
  ticketTypes: [],
  setEvent: (eventId) =>
    set((s) => (s.eventId === eventId ? s : { eventId, lines: [], ticketTypes: [] })),
  setQty: (ticketTypeId, quantity, ticketType) =>
    set((s) => {
      const others = s.lines.filter((l) => l.ticketTypeId !== ticketTypeId);
      const lines = quantity <= 0 ? others : [...others, { ticketTypeId, quantity }];
      
      let ticketTypes = s.ticketTypes;
      if (ticketType && !s.ticketTypes.some((t) => t.id === ticketTypeId)) {
        ticketTypes = [...s.ticketTypes, ticketType];
      }
      return { lines, ticketTypes };
    }),
  clear: () => set({ eventId: null, lines: [], ticketTypes: [] }),
  total: () =>
    get().lines.reduce((sum, l) => {
      const t = get().ticketTypes.find((x) => x.id === l.ticketTypeId);
      return sum + (t ? t.price * l.quantity : 0);
    }, 0),
  checkout: async (method) => {
    const lines = get().lines;
    const items = lines.map((l) => ({
      ticketTypeId: l.ticketTypeId,
      quantity: l.quantity,
    }));

    // 1. Create the order in the Sales API
    const order = await salesApi.createOrder(items);

    // 2. Process the payment in the Sales API
    const payment = await salesApi.payOrder(order.id, method);

    // 3. Clear cart
    get().clear();

    return { 
      order: {
        ...order,
        // Make sure status matches confirmed since we just paid
        status: "confirmed"
      }, 
      ticketCodes: payment.ticketCodes || [] 
    };
  },
}));
