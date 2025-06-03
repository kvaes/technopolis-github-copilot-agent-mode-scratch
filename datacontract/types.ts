// TypeScript interfaces for data contracts

export interface Event {
  id: string;
  name: string;
  location: string;
  date: string; // ISO 8601 date string (YYYY-MM-DD)
  startTime: string; // HH:mm format
  createdAt: string; // ISO 8601 datetime string
  updatedAt: string; // ISO 8601 datetime string
}

export interface Registration {
  id: string;
  eventId: string;
  name: string;
  email: string;
  pronouns?: string; // Optional
  optInCommunication: boolean;
  registeredAt: string; // ISO 8601 datetime string
}

export interface CreateEventRequest {
  name: string;
  location: string;
  date: string; // ISO 8601 date string
  startTime: string; // HH:mm format
}

export interface UpdateEventRequest {
  name?: string;
  location?: string;
  date?: string; // ISO 8601 date string
  startTime?: string; // HH:mm format
}

export interface CreateRegistrationRequest {
  eventId: string;
  name: string;
  email: string;
  pronouns?: string;
  optInCommunication: boolean;
}

export interface EventFilter {
  date?: string; // ISO 8601 date string
  location?: string;
  limit?: number;
  offset?: number;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
  message?: string;
}

export interface EventListResponse {
  events: Event[];
  total: number;
  offset: number;
  limit: number;
}