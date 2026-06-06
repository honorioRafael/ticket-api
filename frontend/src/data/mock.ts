import { Event, Order, Ticket, TicketType, Venue } from "@/types/domain";
import event1 from "@/assets/event-1.jpg";
import event2 from "@/assets/event-2.jpg";
import event3 from "@/assets/event-3.jpg";
import event4 from "@/assets/event-4.jpg";

export const venues: Venue[] = [
  { id: "v1", name: "Fabrik Warehouse", address: "Rua Augusta, 1200 — São Paulo, SP", capacity: 1500 },
  { id: "v2", name: "Museu de Arte Moderna", address: "Av. Infante Dom Henrique, 85 — Rio de Janeiro, RJ", capacity: 3000 },
  { id: "v3", name: "Ópera de Arame", address: "R. João Gava, 970 — Curitiba, PR", capacity: 2400 },
  { id: "v4", name: "Comedy Club Vila", address: "R. Harmonia, 280 — São Paulo, SP", capacity: 200 },
];

export const events: Event[] = [
  {
    id: "e1",
    name: "Midnight Sessions: Techno Archeology",
    description: "Uma noite mergulhada no underground eletrônico com line-up internacional e visuais analógicos.",
    startsAt: "2026-10-24T23:00:00Z",
    endsAt: "2026-10-25T06:00:00Z",
    status: "published",
    venueId: "v1",
    imageUrl: event1,
    category: "Música",
  },
  {
    id: "e2",
    name: "Solar Open Air: Tropicalia Revisitada",
    description: "Festival ao pôr do sol celebrando 60 anos da Tropicália com releituras contemporâneas.",
    startsAt: "2026-11-12T16:00:00Z",
    endsAt: "2026-11-12T23:00:00Z",
    status: "published",
    venueId: "v2",
    imageUrl: event2,
    category: "Festival",
  },
  {
    id: "e3",
    name: "Modern Jazz Quartet — Winter Series",
    description: "Quarteto autoral em apresentação intimista no teatro mais bonito do Brasil.",
    startsAt: "2026-12-05T20:30:00Z",
    endsAt: "2026-12-05T23:00:00Z",
    status: "published",
    venueId: "v3",
    imageUrl: event3,
    category: "Jazz",
  },
  {
    id: "e4",
    name: "Stand Up: Noite Aberta",
    description: "Cinco comediantes, microfone aberto e risadas garantidas até de madrugada.",
    startsAt: "2026-09-18T21:00:00Z",
    endsAt: "2026-09-18T23:30:00Z",
    status: "published",
    venueId: "v4",
    imageUrl: event4,
    category: "Comédia",
  },
];

export const ticketTypes: TicketType[] = [
  { id: "t1", eventId: "e1", name: "Pista", price: 80, totalQuantity: 800, availableQuantity: 312 },
  { id: "t2", eventId: "e1", name: "VIP", price: 180, totalQuantity: 200, availableQuantity: 47 },
  { id: "t3", eventId: "e2", name: "Pista", price: 120, totalQuantity: 1500, availableQuantity: 980 },
  { id: "t4", eventId: "e2", name: "Camarote", price: 320, totalQuantity: 300, availableQuantity: 88 },
  { id: "t5", eventId: "e3", name: "Plateia", price: 95, totalQuantity: 600, availableQuantity: 421 },
  { id: "t6", eventId: "e3", name: "Frisa", price: 220, totalQuantity: 100, availableQuantity: 18 },
  { id: "t7", eventId: "e4", name: "Mesa", price: 60, totalQuantity: 200, availableQuantity: 142 },
];

// Mutable in-memory stores for demo purposes
export const orders: Order[] = [];
export const tickets: Ticket[] = [];

export function getEvent(id: string) {
  return events.find((e) => e.id === id);
}
export function getVenue(id: string) {
  return venues.find((v) => v.id === id);
}
export function getTicketTypesByEvent(eventId: string) {
  return ticketTypes.filter((t) => t.eventId === eventId);
}
export function getTicketType(id: string) {
  return ticketTypes.find((t) => t.id === id);
}

export function formatBRL(value: number) {
  return value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

export function formatDate(iso: string) {
  return new Date(iso).toLocaleDateString("pt-BR", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });
}

export function formatDateTime(iso: string) {
  return new Date(iso).toLocaleString("pt-BR", {
    day: "2-digit",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export function generateTicketCode() {
  return "TK-" + Math.random().toString(36).slice(2, 6).toUpperCase() + "-" + Math.random().toString(36).slice(2, 6).toUpperCase();
}
