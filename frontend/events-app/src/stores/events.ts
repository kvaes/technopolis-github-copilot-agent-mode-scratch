import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { Event, EventFilter, CreateEventRequest, UpdateEventRequest } from '@/types'
import { eventApi, ApiError } from '@/services/api'

export const useEventsStore = defineStore('events', () => {
  const events = ref<Event[]>([])
  const currentEvent = ref<Event | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const totalEvents = ref(0)
  const currentFilter = ref<EventFilter>({})

  const filteredEvents = computed(() => {
    let filtered = events.value

    if (currentFilter.value.date) {
      filtered = filtered.filter(event => event.date === currentFilter.value.date)
    }

    if (currentFilter.value.location) {
      filtered = filtered.filter(event => 
        event.location.toLowerCase().includes(currentFilter.value.location!.toLowerCase())
      )
    }

    return filtered
  })

  async function fetchEvents(filter?: EventFilter) {
    loading.value = true
    error.value = null
    
    try {
      if (filter) {
        currentFilter.value = { ...filter }
      }
      
      const response = await eventApi.getEvents(filter)
      events.value = response.events
      totalEvents.value = response.total
    } catch (err) {
      error.value = err instanceof ApiError ? err.message : 'Failed to fetch events'
      console.error('Error fetching events:', err)
    } finally {
      loading.value = false
    }
  }

  async function fetchEvent(id: string) {
    loading.value = true
    error.value = null
    
    try {
      const event = await eventApi.getEvent(id)
      currentEvent.value = event
      return event
    } catch (err) {
      error.value = err instanceof ApiError ? err.message : 'Failed to fetch event'
      console.error('Error fetching event:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createEvent(eventData: CreateEventRequest) {
    loading.value = true
    error.value = null
    
    try {
      const newEvent = await eventApi.createEvent(eventData)
      events.value.unshift(newEvent)
      totalEvents.value += 1
      return newEvent
    } catch (err) {
      error.value = err instanceof ApiError ? err.message : 'Failed to create event'
      console.error('Error creating event:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateEvent(id: string, eventData: UpdateEventRequest) {
    loading.value = true
    error.value = null
    
    try {
      const updatedEvent = await eventApi.updateEvent(id, eventData)
      const index = events.value.findIndex(e => e.id === id)
      if (index !== -1) {
        events.value[index] = updatedEvent
      }
      if (currentEvent.value?.id === id) {
        currentEvent.value = updatedEvent
      }
      return updatedEvent
    } catch (err) {
      error.value = err instanceof ApiError ? err.message : 'Failed to update event'
      console.error('Error updating event:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteEvent(id: string) {
    loading.value = true
    error.value = null
    
    try {
      await eventApi.deleteEvent(id)
      events.value = events.value.filter(e => e.id !== id)
      totalEvents.value -= 1
      if (currentEvent.value?.id === id) {
        currentEvent.value = null
      }
    } catch (err) {
      error.value = err instanceof ApiError ? err.message : 'Failed to delete event'
      console.error('Error deleting event:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  function clearError() {
    error.value = null
  }

  function clearCurrentEvent() {
    currentEvent.value = null
  }

  return {
    events,
    currentEvent,
    loading,
    error,
    totalEvents,
    currentFilter,
    filteredEvents,
    fetchEvents,
    fetchEvent,
    createEvent,
    updateEvent,
    deleteEvent,
    clearError,
    clearCurrentEvent
  }
})
