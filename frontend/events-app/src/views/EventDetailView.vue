<template>
  <main>
    <div class="event-detail-page">
      <!-- Loading State -->
      <div v-if="loading" class="loading">
        <p>Loading event...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="error-message">
        {{ error }}
        <RouterLink to="/events" class="btn btn-primary">Back to Events</RouterLink>
      </div>

      <!-- Event Details -->
      <div v-else-if="currentEvent" class="event-detail">
        <header class="event-header">
          <h1>{{ currentEvent.name }}</h1>
          <div class="event-meta">
            <p class="location">📍 {{ currentEvent.location }}</p>
            <p class="date-time">
              📅 {{ formatDate(currentEvent.date) }} at {{ currentEvent.startTime }}
            </p>
          </div>
        </header>

        <div class="event-actions">
          <RouterLink to="/events" class="btn btn-secondary">← Back to Events</RouterLink>
          <button @click="showRegistrationForm = true" class="btn btn-primary">
            Register for Event
          </button>
        </div>

        <!-- Registration Form Modal -->
        <div v-if="showRegistrationForm" class="modal-overlay" @click="closeRegistrationForm">
          <div class="modal-content" @click.stop>
            <h2>Register for {{ currentEvent.name }}</h2>
            
            <form @submit.prevent="submitRegistration" class="registration-form">
              <div class="form-group">
                <label for="name">Name *</label>
                <input
                  id="name"
                  v-model="registrationForm.name"
                  type="text"
                  required
                  :disabled="submittingRegistration"
                />
              </div>

              <div class="form-group">
                <label for="email">Email *</label>
                <input
                  id="email"
                  v-model="registrationForm.email"
                  type="email"
                  required
                  :disabled="submittingRegistration"
                />
              </div>

              <div class="form-group">
                <label for="pronouns">Pronouns</label>
                <input
                  id="pronouns"
                  v-model="registrationForm.pronouns"
                  type="text"
                  placeholder="e.g., he/him, she/her, they/them"
                  :disabled="submittingRegistration"
                />
              </div>

              <div class="form-group checkbox-group">
                <label class="checkbox-label">
                  <input
                    v-model="registrationForm.optInCommunication"
                    type="checkbox"
                    :disabled="submittingRegistration"
                  />
                  I would like to receive further communications about events
                </label>
              </div>

              <div v-if="registrationError" class="error-message">
                {{ registrationError }}
              </div>

              <div class="form-actions">
                <button
                  type="button"
                  @click="closeRegistrationForm"
                  class="btn btn-secondary"
                  :disabled="submittingRegistration"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  class="btn btn-primary"
                  :disabled="submittingRegistration"
                >
                  {{ submittingRegistration ? 'Registering...' : 'Register' }}
                </button>
              </div>
            </form>
          </div>
        </div>

        <!-- Success Message -->
        <div v-if="registrationSuccess" class="success-message">
          <h3>Registration Successful!</h3>
          <p>You have successfully registered for {{ currentEvent.name }}.</p>
          <button @click="registrationSuccess = false" class="close-btn">&times;</button>
        </div>
      </div>
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useEventsStore } from '@/stores/events'
import { registrationApi, ApiError } from '@/services/api'
import type { CreateRegistrationRequest } from '@/types'

const route = useRoute()
const eventsStore = useEventsStore()

const { currentEvent, loading, error, fetchEvent, clearError } = eventsStore

const showRegistrationForm = ref(false)
const registrationForm = ref<CreateRegistrationRequest>({
  eventId: '',
  name: '',
  email: '',
  pronouns: '',
  optInCommunication: false,
})
const submittingRegistration = ref(false)
const registrationError = ref<string | null>(null)
const registrationSuccess = ref(false)

onMounted(async () => {
  const eventId = route.params.id as string
  if (eventId) {
    try {
      await fetchEvent(eventId)
      registrationForm.value.eventId = eventId
    } catch (err) {
      console.error('Failed to load event:', err)
    }
  }
})

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
}

function closeRegistrationForm() {
  showRegistrationForm.value = false
  registrationError.value = null
  // Reset form
  registrationForm.value = {
    eventId: route.params.id as string,
    name: '',
    email: '',
    pronouns: '',
    optInCommunication: false,
  }
}

async function submitRegistration() {
  submittingRegistration.value = true
  registrationError.value = null

  try {
    await registrationApi.createRegistration(registrationForm.value)
    registrationSuccess.value = true
    closeRegistrationForm()
  } catch (err) {
    registrationError.value = err instanceof ApiError ? err.message : 'Failed to register for event'
    console.error('Registration failed:', err)
  } finally {
    submittingRegistration.value = false
  }
}
</script>

<style scoped>
.event-detail-page {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}

.loading {
  text-align: center;
  padding: 2rem;
  color: var(--color-text);
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

.success-message {
  background-color: #d4edda;
  color: #155724;
  padding: 1rem;
  border-radius: 0.5rem;
  margin-bottom: 2rem;
  position: relative;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
}

.event-detail {
  background-color: var(--color-background-soft);
  border-radius: 0.5rem;
  padding: 2rem;
}

.event-header h1 {
  font-size: 2.5rem;
  margin-bottom: 1rem;
  color: var(--color-heading);
}

.event-meta {
  margin-bottom: 2rem;
}

.event-meta p {
  font-size: 1.1rem;
  margin-bottom: 0.5rem;
  color: var(--color-text);
}

.event-actions {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 0.5rem;
  padding: 2rem;
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-content h2 {
  margin-bottom: 1.5rem;
  color: var(--color-heading);
}

.registration-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
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
}

.form-group input:focus {
  outline: none;
  border-color: var(--color-border-hover);
}

.checkbox-group {
  flex-direction: row;
  align-items: center;
  gap: 0.5rem;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: normal;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 1rem;
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