import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { Toaster } from "@/components/ui/toaster";
import { TooltipProvider } from "@/components/ui/tooltip";
import Index from "./pages/Index.tsx";
import NotFound from "./pages/NotFound.tsx";
import EventDetail from "./pages/EventDetail.tsx";
import Checkout from "./pages/Checkout.tsx";
import Success from "./pages/Success.tsx";
import MyTickets from "./pages/MyTickets.tsx";
import Admin from "./pages/Admin.tsx";
import Login from "./pages/Login.tsx";
import Signup from "./pages/Signup.tsx";
import EventTickets from "./pages/EventTickets.tsx";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Index />} />
          <Route path="/evento/:id" element={<EventDetail />} />
          <Route path="/evento/:id/ingressos" element={<EventTickets />} />
          <Route path="/login" element={<Login />} />
          <Route path="/criar-conta" element={<Signup />} />
          <Route path="/checkout" element={<Checkout />} />
          <Route path="/sucesso/:orderId" element={<Success />} />
          <Route path="/meus-ingressos" element={<MyTickets />} />
          <Route path="/admin" element={<Admin />} />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </BrowserRouter>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
