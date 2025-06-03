<template>
  <main>
    <div class="events-page">
      <header class="page-header">
        <h1>Events</h1>
        <RouterLink to="/events/create" class="btn btn-primary">Create Event</RouterLink>
      </header>

      <!-- Filter Section -->
      <div class="filters">
        <div class="filter-group">
          <label for="date-filter">Date:</label>
          <input
            id="date-filter"
            v-model="filters.date"
            type="date"
            @change="applyFilters"
          />
        </div>
        <div class="filter-group">
          <label for="location-filter">Location:</label>
          <input
            id="location-filter"
            v-model="filters.location"
            type="text"
            placeholder="Enter location..."
            @input="applyFilters"
          />
        </div>
        <button @click="clearFilters" class="btn btn-secondary">Clear Filters</button>
      </div>

      <!-- Error Message -->
      <div v-if="error" class="error-message">
        {{ error }}
        <button @click="clearError" class="close-btn">&times;</button>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="loading">
        <p>Loading events...</p>
      </div>

      <!-- Events List -->
      <div v-else-if="events.length > 0" class="events-grid">
        <div
          v-for="event in events"
          :key="event.id"
          class="event-card"
          @click="goToEvent(event.id)"
        >
          <h3>{{ event.name }}</h3>
          <p class="location">📍 {{ event.location }}</p>
          <p class="date-time">📅 {{ formatDate(event.date) }} at {{ event.startTime }}</p>
          <div class="card-actions">
            <button @click.stop="editEvent(event.id)" class="btn btn-sm btn-secondary">Edit</button>
            <button @click.stop="deleteEventHandler(event.id)" class="btn btn-sm btn-danger">Delete</button>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="empty-state">
        <h3>No events found</h3>
        <p>Be the first to create an event!</p>
        <RouterLink to="/events/create" class="btn btn-primary">Create Event</RouterLink>
      </div>
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useEventsStore } from '@/stores/events'
import type { EventFilter } from '@/types'

const router = useRouter()
const eventsStore = useEventsStore()

const { events, loading, error, fetchEvents, deleteEvent, clearError } = eventsStore

const filters = ref<EventFilter>({
  date: '',
  location: '',
})

onMounted(() => {
  fetchEvents()
})

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString()
}

function applyFilters() {
  const filterParams: EventFilter = {}
  
  if (filters.value.date) {
    filterParams.date = filters.value.date
  }
  
  if (filters.value.location?.trim()) {
    filterParams.location = filters.value.location.trim()
  }
  
  fetchEvents(filterParams)
}

function clearFilters() {
  filters.value.date = ''
  filters.value.location = ''
  fetchEvents()
}

function goToEvent(id: string) {
  router.push(`/events/${id}`)
}

function editEvent(id: string) {
  // For now, just go to the event detail page
  // In a full implementation, this could open an edit modal or go to an edit page
  router.push(`/events/${id}`)
}

async function deleteEventHandler(id: string) {
  if (confirm('Are you sure you want to delete this event?')) {
    try {
      await deleteEvent(id)
    } catch (err) {
      console.error('Failed to delete event:', err)
    }
  }
}
</script>

<style scoped>
.events-page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2.5rem;
  color: var(--color-heading);
}

.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  padding: 1rem;
  background-color: var(--color-background-soft);
  border-radius: 0.5rem;
  flex-wrap: wrap;
  align-items: end;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.filter-group label {
  font-weight: 600;
  color: var(--color-text);
}

.filter-group input {
  padding: 0.5rem;
  border: 1px solid var(--color-border);
  border-radius: 0.25rem;
  font-size: 1rem;
}

.error-message {
  background-color: #fee;
  color: #c33;
  padding: 1rem;
  border-radius: 0.5rem;
  margin-bottom: 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #c33;
}

.loading {
  text-align: center;
  padding: 2rem;
  color: var(--color-text);
}

.events-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.event-card {
  background-color: var(--color-background-soft);
  border: 1px solid var(--color-border);
  border-radius: 0.5rem;
  padding: 1.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.event-card:hover {
  border-color: var(--color-border-hover);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.event-card h3 {
  font-size: 1.25rem;
  margin-bottom: 0.5rem;
  color: var(--color-heading);
}

.event-card .location,
.event-card .date-time {
  margin-bottom: 0.5rem;
  color: var(--color-text);
}

.card-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 1rem;
}

.empty-state {
  text-align: center;
  padding: 4rem 2rem;
}

.empty-state h3 {
  font-size: 1.5rem;
  margin-bottom: 1rem;
  color: var(--color-heading);
}

.empty-state p {
  margin-bottom: 2rem;
  color: var(--color-text);
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.3s ease;
  display: inline-block;
}

.btn-sm {
  padding: 0.25rem 0.5rem;
  font-size: 0.875rem;
}

.btn-primary {
  background-color: var(--color-border-hover);
  color: white;
}

.btn-primary:hover {
  background-color: var(--color-border);
}

.btn-secondary {
  background-color: transparent;
  color: var(--color-border-hover);
  border: 1px solid var(--color-border-hover);
}

.btn-secondary:hover {
  background-color: var(--color-border-hover);
  color: white;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn-danger:hover {
  background-color: #c82333;
}
</style>