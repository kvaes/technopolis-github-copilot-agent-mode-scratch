import type { 
  Event, 
  Registration, 
  CreateEventRequest, 
  UpdateEventRequest, 
  CreateRegistrationRequest, 
  EventFilter, 
  ApiResponse, 
  EventListResponse 
} from '@/types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:7071/api';

class ApiError extends Error {
  constructor(message: string, public status?: number) {
    super(message);
    this.name = 'ApiError';
  }
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const text = await response.text();
    let errorMessage = `HTTP ${response.status}: ${response.statusText}`;
    
    try {
      const errorData = JSON.parse(text);
      if (errorData.error) {
        errorMessage = errorData.error;
      }
    } catch {
      // If not JSON, use the text as error message
      if (text) {
        errorMessage = text;
      }
    }
    
    throw new ApiError(errorMessage, response.status);
  }

  const data = await response.json();
  return data;
}

export const eventApi = {
  async getEvents(filter?: EventFilter): Promise<EventListResponse> {
    const params = new URLSearchParams();
    if (filter?.date) params.append('date', filter.date);
    if (filter?.location) params.append('location', filter.location);
    if (filter?.limit) params.append('limit', filter.limit.toString());
    if (filter?.offset) params.append('offset', filter.offset.toString());

    const url = `${API_BASE_URL}/events${params.toString() ? '?' + params.toString() : ''}`;
    const response = await fetch(url);
    const apiResponse = await handleResponse<ApiResponse<EventListResponse>>(response);
    
    if (!apiResponse.success || !apiResponse.data) {
      throw new ApiError(apiResponse.error || 'Failed to fetch events');
    }
    
    return apiResponse.data;
  },

  async getEvent(id: string): Promise<Event> {
    const response = await fetch(`${API_BASE_URL}/events/${id}`);
    const apiResponse = await handleResponse<ApiResponse<Event>>(response);
    
    if (!apiResponse.success || !apiResponse.data) {
      throw new ApiError(apiResponse.error || 'Failed to fetch event');
    }
    
    return apiResponse.data;
  },

  async createEvent(event: CreateEventRequest): Promise<Event> {
    const response = await fetch(`${API_BASE_URL}/events`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(event),
    });
    
    const apiResponse = await handleResponse<ApiResponse<Event>>(response);
    
    if (!apiResponse.success || !apiResponse.data) {
      throw new ApiError(apiResponse.error || 'Failed to create event');
    }
    
    return apiResponse.data;
  },

  async updateEvent(id: string, event: UpdateEventRequest): Promise<Event> {
    const response = await fetch(`${API_BASE_URL}/events/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(event),
    });
    
    const apiResponse = await handleResponse<ApiResponse<Event>>(response);
    
    if (!apiResponse.success || !apiResponse.data) {
      throw new ApiError(apiResponse.error || 'Failed to update event');
    }
    
    return apiResponse.data;
  },

  async deleteEvent(id: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/events/${id}`, {
      method: 'DELETE',
    });
    
    const apiResponse = await handleResponse<ApiResponse<void>>(response);
    
    if (!apiResponse.success) {
      throw new ApiError(apiResponse.error || 'Failed to delete event');
    }
  },
};

export const registrationApi = {
  async createRegistration(registration: CreateRegistrationRequest): Promise<Registration> {
    const response = await fetch(`${API_BASE_URL}/registrations`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(registration),
    });
    
    const apiResponse = await handleResponse<ApiResponse<Registration>>(response);
    
    if (!apiResponse.success || !apiResponse.data) {
      throw new ApiError(apiResponse.error || 'Failed to create registration');
    }
    
    return apiResponse.data;
  },

  async getRegistrationsByEvent(eventId: string): Promise<Registration[]> {
    const response = await fetch(`${API_BASE_URL}/events/${eventId}/registrations`);
    const apiResponse = await handleResponse<ApiResponse<Registration[]>>(response);
    
    if (!apiResponse.success || !apiResponse.data) {
      throw new ApiError(apiResponse.error || 'Failed to fetch registrations');
    }
    
    return apiResponse.data;
  },
};

export { ApiError };