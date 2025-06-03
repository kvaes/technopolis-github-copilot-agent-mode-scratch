<template>
  <main>
    <div class="create-event-page">
      <header class="page-header">
        <h1>Create New Event</h1>
        <RouterLink to="/events" class="btn btn-secondary">← Back to Events</RouterLink>
      </header>

      <div class="form-container">
        <form @submit.prevent="submitEvent" class="event-form">
          <div class="form-group">
            <label for="name">Event Name *</label>
            <input
              id="name"
              v-model="eventForm.name"
              type="text"
              required
              :disabled="submitting"
              placeholder="Enter event name..."
            />
          </div>

          <div class="form-group">
            <label for="location">Location *</label>
            <input
              id="location"
              v-model="eventForm.location"
              type="text"
              required
              :disabled="submitting"
              placeholder="Enter event location..."
            />
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="date">Date *</label>
              <input
                id="date"
                v-model="eventForm.date"
                type="date"
                required
                :disabled="submitting"
                :min="today"
              />
            </div>

            <div class="form-group">
              <label for="startTime">Start Time *</label>
              <input
                id="startTime"
                v-model="eventForm.startTime"
                type="time"
                required
                :disabled="submitting"
              />
            </div>
          </div>

          <div v-if="error" class="error-message">
            {{ error }}
          </div>

          <div class="form-actions">
            <button
              type="button"
              @click="resetForm"
              class="btn btn-secondary"
              :disabled="submitting"
            >
              Reset
            </button>
            <button
              type="submit"
              class="btn btn-primary"
              :disabled="submitting || !isFormValid"
            >
              {{ submitting ? 'Creating...' : 'Create Event' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useEventsStore } from '@/stores/events'
import type { CreateEventRequest } from '@/types'
import { ApiError } from '@/services/api'

const router = useRouter()
const eventsStore = useEventsStore()

const { createEvent, clearError } = eventsStore

const eventForm = ref<CreateEventRequest>({
  name: '',
  location: '',
  date: '',
  startTime: '',
})

const submitting = ref(false)
const error = ref<string | null>(null)

const today = computed(() => {
  return new Date().toISOString().split('T')[0]
})

const isFormValid = computed(() => {
  return (
    eventForm.value.name.trim() &&
    eventForm.value.location.trim() &&
    eventForm.value.date &&
    eventForm.value.startTime
  )
})

function resetForm() {
  eventForm.value = {
    name: '',
    location: '',
    date: '',
    startTime: '',
  }
  error.value = null
}

async function submitEvent() {
  if (!isFormValid.value) {
    error.value = 'Please fill in all required fields'
    return
  }

  submitting.value = true
  error.value = null

  try {
    const newEvent = await createEvent(eventForm.value)
    
    // Success - redirect to the new event's detail page
    router.push(`/events/${newEvent.id}`)
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Failed to create event'
    console.error('Failed to create event:', err)
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.create-event-page {
  max-width: 600px;
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

.form-container {
  background-color: var(--color-background-soft);
  border-radius: 0.5rem;
  padding: 2rem;
}

.event-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 600;
  color: var(--color-text);
}

.form-group input {
  padding: 0.75rem;
  border: 1px solid var(--color-border);
  border-radius: 0.25rem;
  font-size: 1rem;
  transition: border-color 0.3s ease;
}

.form-group input:focus {
  outline: none;
  border-color: var(--color-border-hover);
}

.form-group input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

@media (max-width: 600px) {
  .form-row {
    grid-template-columns: 1fr;
  }
}

.error-message {
  background-color: #fee;
  color: #c33;
  padding: 1rem;
  border-radius: 0.25rem;
  border: 1px solid #fcc;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 1rem;
}

@media (max-width: 600px) {
  .form-actions {
    flex-direction: column;
  }
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.3s ease;
  display: inline-block;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background-color: var(--color-border-hover);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: var(--color-border);
}

.btn-secondary {
  background-color: transparent;
  color: var(--color-border-hover);
  border: 1px solid var(--color-border-hover);
}

.btn-secondary:hover:not(:disabled) {
  background-color: var(--color-border-hover);
  color: white;
}
</style>