// Domain types — Sistema de Compra de Ingressos
// Bounded contexts: Events & Sales

export type EventStatus = "draft" | "published" | "cancelled" | "finished";
export type OrderStatus = "pending" | "confirmed" | "cancelled";
export type TicketStatus = "active" | "used" | "cancelled";
export type PaymentMethod = "credit_card" | "pix" | "boleto";
export type PaymentStatus = "pending" | "paid" | "failed" | "refunded";

export interface Venue {
  id: string;
  name: string;
  address: string;
  capacity: number;
}

export interface Event {
  id: string;
  name: string;
  description?: string;
  startsAt: string;
  endsAt: string;
  status: EventStatus;
  venueId: string;
  imageUrl?: string;
  category?: string;
}

export interface TicketType {
  id: string;
  eventId: string;
  name: string;
  price: number;
  totalQuantity: number;
  availableQuantity: number;
}

export interface Customer {
  id: string;
  name: string;
  email: string;
  document: string;
}

export interface OrderItem {
  id: string;
  orderId: string;
  ticketTypeId: string;
  unitPrice: number;
  quantity: number;
}

export interface Order {
  id: string;
  customerId: string;
  placedAt: string;
  totalAmount: number;
  status: OrderStatus;
  items: OrderItem[];
}

export interface Ticket {
  id: string;
  orderItemId: string;
  code: string;
  status: TicketStatus;
}

export interface Payment {
  id: string;
  orderId: string;
  method: PaymentMethod;
  status: PaymentStatus;
  amount: number;
  paidAt?: string;
}
