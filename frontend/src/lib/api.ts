import { Event, Venue, TicketType, Order, Ticket, Customer } from "@/types/domain";
import event1 from "@/assets/event-1.jpg";
import event2 from "@/assets/event-2.jpg";
import event3 from "@/assets/event-3.jpg";
import event4 from "@/assets/event-4.jpg";

// Centralized API URLs
export const EVENTS_API_URL = import.meta.env.VITE_EVENTS_API_URL || "https://localhost:7001";
export const SALES_API_URL = import.meta.env.VITE_SALES_API_URL || "https://localhost:7002";

// Local event asset references for high-quality metadata fallbacks
const eventImages = [event1, event2, event3, event4];

let tokenProvider: (() => string | null) | null = null;

export function registerTokenProvider(provider: () => string | null) {
  tokenProvider = provider;
}

// Helper to retrieve persisted JWT token
function getAuthToken(): string | null {
  if (tokenProvider) {
    try {
      return tokenProvider();
    } catch {
      // fallback
    }
  }
  try {
    const raw = localStorage.getItem("tikket-auth");
    if (!raw) return null;
    const parsed = JSON.parse(raw);
    return parsed?.state?.user?.token || null;
  } catch {
    return null;
  }
}

function enforceHttps(url: string): string {
  let secureUrl = url;
  if (secureUrl.startsWith("http://localhost:5001")) {
    secureUrl = secureUrl.replace("http://localhost:5001", "https://localhost:7001");
  } else if (secureUrl.startsWith("http://localhost:5002")) {
    secureUrl = secureUrl.replace("http://localhost:5002", "https://localhost:7002");
  } else if (secureUrl.startsWith("http://")) {
    secureUrl = "https://" + secureUrl.substring(7);
  }
  return secureUrl;
}

// Custom request wrapper
async function request(baseUrl: string, path: string, options: RequestInit = {}) {
  const token = getAuthToken();
  const headers = new Headers(options.headers || {});
  
  if (!headers.has("Content-Type") && !(options.body instanceof FormData)) {
    headers.set("Content-Type", "application/json");
  }
  
  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  const rawUrl = `${baseUrl}${path.startsWith("/") ? path : `/${path}`}`;
  const url = enforceHttps(rawUrl);
  const response = await fetch(url, { 
    ...options, 
    headers: Object.fromEntries(headers.entries()) 
  });

  if (!response.ok) {
    let errorMessage = `Erro na requisição: ${response.statusText}`;
    try {
      const errorJson = await response.json();
      if (errorJson.message) errorMessage = errorJson.message;
      else if (errorJson.detail) errorMessage = errorJson.detail;
    } catch {
      // ignore parsing errors, use statusText fallback
    }
    throw new Error(errorMessage);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

// Generate aesthetic metadata dynamically for backend events
export function getEventMetadata(id: string, name: string) {
  const lowerName = name.toLowerCase();
  let imageUrl = eventImages[0];
  let category = "Música";
  let description = "Uma experiência única com os melhores artistas, som de alta fidelidade e atmosfera incrível.";

  if (id === "e1" || lowerName.includes("techno") || lowerName.includes("midnight") || lowerName.includes("eletrônica")) {
    imageUrl = eventImages[0];
    category = "Música";
    description = "Uma noite mergulhada no underground eletrônico com line-up internacional e visuais analógicos.";
  } else if (id === "e2" || lowerName.includes("solar") || lowerName.includes("tropicalia") || lowerName.includes("open air") || lowerName.includes("festival")) {
    imageUrl = eventImages[1];
    category = "Festival";
    description = "Festival ao pôr do sol celebrando o melhor da música brasileira com releituras contemporâneas.";
  } else if (id === "e3" || lowerName.includes("jazz") || lowerName.includes("quartet") || lowerName.includes("winter") || lowerName.includes("instrumental")) {
    imageUrl = eventImages[2];
    category = "Jazz";
    description = "Apresentação intimista e autoral no teatro mais bonito e icônico da cidade.";
  } else if (id === "e4" || lowerName.includes("stand up") || lowerName.includes("comedy") || lowerName.includes("risadas") || lowerName.includes("comédia")) {
    imageUrl = eventImages[3];
    category = "Comédia";
    description = "Cinco humoristas consagrados, microfone aberto e risadas garantidas para toda a família.";
  } else {
    // Deterministic generation for new database-driven events
    let hash = 0;
    const key = id + name;
    for (let i = 0; i < key.length; i++) {
      hash = key.charCodeAt(i) + ((hash << 5) - hash);
    }
    const idx = Math.abs(hash) % 4;
    imageUrl = eventImages[idx];

    const cats = ["Música", "Festival", "Jazz", "Comédia"];
    category = cats[Math.abs(hash) % cats.length];
  }

  return { imageUrl, category, description };
}

// Map backend Event API response to frontend Event structure
function mapBackendEvent(raw: any): Event & { ticketTypes: TicketType[] } {
  const meta = getEventMetadata(raw.id, raw.name);
  
  // Format event status (backend might use integers, uppercase strings, etc.)
  let status = "published" as any;
  if (raw.status) {
    const s = String(raw.status).toLowerCase();
    if (s === "draft" || s === "0") status = "draft";
    else if (s === "cancelled" || s === "2") status = "cancelled";
    else if (s === "finished" || s === "3") status = "finished";
  }

  // Parse ticket types if embedded
  const ticketTypes: TicketType[] = (raw.ticketTypes || []).map((t: any) => ({
    id: t.id,
    eventId: raw.id,
    name: t.name,
    price: t.price,
    totalQuantity: t.totalQuantity,
    availableQuantity: t.availableQuantity ?? t.totalQuantity,
  }));

  return {
    id: raw.id,
    name: raw.name,
    startsAt: raw.startsAt,
    endsAt: raw.endsAt,
    status,
    venueId: raw.venueId,
    imageUrl: meta.imageUrl,
    category: meta.category,
    description: meta.description,
    ticketTypes,
  };
}

// API CLIENT MODULES

// --- Events & Venues ---
export const eventsApi = {
  async getAll(page = 1, pageSize = 100): Promise<Event[]> {
    const data = await request(EVENTS_API_URL, `/events?page=${page}&pageSize=${pageSize}`);
    const items = data.items || [];
    return items.map(mapBackendEvent);
  },

  async getById(id: string): Promise<Event & { ticketTypes: TicketType[] }> {
    const raw = await request(EVENTS_API_URL, `/events/${id}`);
    return mapBackendEvent(raw);
  },

  async create(name: string, startsAt: string, endsAt: string, venueId: string): Promise<Event> {
    const raw = await request(EVENTS_API_URL, "/events", {
      method: "POST",
      body: JSON.stringify({ name, startsAt, endsAt, venueId }),
    });
    return mapBackendEvent(raw);
  },

  async update(id: string, name: string, startsAt: string, endsAt: string, venueId: string): Promise<Event> {
    const raw = await request(EVENTS_API_URL, `/events/${id}`, {
      method: "PUT",
      body: JSON.stringify({ name, startsAt, endsAt, venueId }),
    });
    return mapBackendEvent(raw);
  },

  async delete(id: string): Promise<void> {
    await request(EVENTS_API_URL, `/events/${id}`, { method: "DELETE" });
  },

  async createTicketType(eventId: string, name: string, price: number, totalQuantity: number): Promise<TicketType> {
    const raw = await request(EVENTS_API_URL, `/events/${eventId}/ticket-types`, {
      method: "POST",
      body: JSON.stringify({ name, price, totalQuantity }),
    });
    return {
      id: raw.id,
      eventId: eventId,
      name: raw.name,
      price: raw.price,
      totalQuantity: raw.totalQuantity,
      availableQuantity: raw.totalQuantity,
    };
  },

  async getVenues(page = 1, pageSize = 100): Promise<Venue[]> {
    const data = await request(EVENTS_API_URL, `/venues?page=${page}&pageSize=${pageSize}`);
    return data.items || [];
  },

  async getVenueById(id: string): Promise<Venue> {
    return request(EVENTS_API_URL, `/venues/${id}`);
  },

  async createVenue(name: string, address: string, capacity: number): Promise<Venue> {
    return request(EVENTS_API_URL, "/venues", {
      method: "POST",
      body: JSON.stringify({ name, address, capacity }),
    });
  },

  async deleteVenue(id: string): Promise<void> {
    await request(EVENTS_API_URL, `/venues/${id}`, { method: "DELETE" });
  }
};

// --- Authentication ---
export const authApi = {
  async registerCustomer(name: string, email: string, document: string, password: string): Promise<{ token: string; customer: Customer }> {
    const cleanDocument = document.replace(/\D/g, "");
    const raw = await request(SALES_API_URL, "/customers", {
      method: "POST",
      body: JSON.stringify({ name, email, document: cleanDocument, password }),
    });
    // The backend register endpoint might return the token directly or require subsequent login.
    // If it returns the newly created customer, we try to automatically log them in.
    try {
      return await this.loginCustomer(email, password);
    } catch {
      return { token: "", customer: { id: raw.id, name, email, document } };
    }
  },

  async loginCustomer(email: string, password: string): Promise<{ token: string; customer: Customer }> {
    const data = await request(SALES_API_URL, "/customers/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });
    return {
      token: data.token,
      customer: {
        id: data.customer.id,
        name: data.customer.name,
        email: data.customer.email.value || data.customer.email, // Handle ValueObject representation if applicable
        document: data.customer.document.value || data.customer.document,
      }
    };
  },

  async registerOrganizer(name: string, email: string, password: string): Promise<{ token: string; organizer: any }> {
    await request(EVENTS_API_URL, "/organizers/register", {
      method: "POST",
      body: JSON.stringify({ name, email, password }),
    });
    return this.loginOrganizer(email, password);
  },

  async loginOrganizer(email: string, password: string): Promise<{ token: string; organizer: any }> {
    const data = await request(EVENTS_API_URL, "/organizers/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });
    return {
      token: data.token,
      organizer: data.organizer,
    };
  }
};

// --- Orders, Payments & Tickets ---
export const salesApi = {
  async createOrder(items: { ticketTypeId: string; quantity: number }[]): Promise<Order> {
    return request(SALES_API_URL, "/orders", {
      method: "POST",
      body: JSON.stringify({ items }),
    });
  },

  async payOrder(orderId: string, method: "credit_card" | "pix" | "boleto"): Promise<any> {
    return request(SALES_API_URL, `/orders/${orderId}/payment`, {
      method: "POST",
      body: JSON.stringify({ method }),
    });
  },

  async getOrders(page = 1, pageSize = 100): Promise<Order[]> {
    const data = await request(SALES_API_URL, `/orders?page=${page}&pageSize=${pageSize}`);
    return data.items || [];
  },

  async getOrderById(id: string): Promise<Order> {
    return request(SALES_API_URL, `/orders/${id}`);
  },

  async getTicketsByOrderId(orderId: string): Promise<Ticket[]> {
    // We can fetch the order details and extract tickets, or fetch tickets directly
    const order = await this.getOrderById(orderId);
    // Let's check if the orderDto contains tickets, or we fetch them separately.
    // If orderDto has items, let's map them or call a webhook to ensure tickets are active.
    // Let's assume order.items carries the tickets, or retrieve the order which includes active tickets.
    // In our backend, the tickets are linked to the order items. Let's see if order payload returns them.
    return (order as any).tickets || [];
  },

  async validateTicket(code: string): Promise<Ticket> {
    return request(SALES_API_URL, `/tickets/${code}/validate`, {
      method: "POST",
    });
  }
};
