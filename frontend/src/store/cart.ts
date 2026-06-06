import { create } from "zustand";
import { Order, OrderItem, Ticket } from "@/types/domain";
import { generateTicketCode, getTicketType, orders, tickets } from "@/data/mock";

interface CartLine {
  ticketTypeId: string;
  quantity: number;
}

interface CartState {
  eventId: string | null;
  lines: CartLine[];
  setEvent: (eventId: string) => void;
  setQty: (ticketTypeId: string, quantity: number) => void;
  clear: () => void;
  total: () => number;
  checkout: (customerId: string, method: "credit_card" | "pix" | "boleto") => { order: Order; tickets: Ticket[] };
}

export const useCart = create<CartState>((set, get) => ({
  eventId: null,
  lines: [],
  setEvent: (eventId) =>
    set((s) => (s.eventId === eventId ? s : { eventId, lines: [] })),
  setQty: (ticketTypeId, quantity) =>
    set((s) => {
      const others = s.lines.filter((l) => l.ticketTypeId !== ticketTypeId);
      if (quantity <= 0) return { lines: others };
      return { lines: [...others, { ticketTypeId, quantity }] };
    }),
  clear: () => set({ eventId: null, lines: [] }),
  total: () =>
    get().lines.reduce((sum, l) => {
      const t = getTicketType(l.ticketTypeId);
      return sum + (t ? t.price * l.quantity : 0);
    }, 0),
  checkout: (customerId, method) => {
    const lines = get().lines;
    const orderId = "o" + Date.now();
    const items: OrderItem[] = lines.map((l, i) => {
      const t = getTicketType(l.ticketTypeId)!;
      return {
        id: `${orderId}-i${i}`,
        orderId,
        ticketTypeId: t.id,
        unitPrice: t.price,
        quantity: l.quantity,
      };
    });
    const totalAmount = items.reduce((s, i) => s + i.unitPrice * i.quantity, 0);
    const order: Order = {
      id: orderId,
      customerId,
      placedAt: new Date().toISOString(),
      totalAmount,
      status: "confirmed",
      items,
    };
    const issued: Ticket[] = [];
    items.forEach((item) => {
      for (let q = 0; q < item.quantity; q++) {
        issued.push({
          id: `${item.id}-t${q}`,
          orderItemId: item.id,
          code: generateTicketCode(),
          status: "active",
        });
      }
      // decrement availability
      const tt = getTicketType(item.ticketTypeId);
      if (tt) tt.availableQuantity = Math.max(0, tt.availableQuantity - item.quantity);
    });
    orders.unshift(order);
    tickets.unshift(...issued);
    // payment is created implicitly as paid for demo
    void method;
    return { order, tickets: issued };
  },
}));
