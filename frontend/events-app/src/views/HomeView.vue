<template>
  <main>
    <div class="hero">
      <h1>Event Management System</h1>
      <p>Discover and manage events in your area</p>
      <div class="actions">
        <RouterLink to="/events" class="btn btn-primary">View Events</RouterLink>
        <RouterLink to="/events/create" class="btn btn-secondary">Create Event</RouterLink>
      </div>
    </div>
  </main>
</template>

<style scoped>
.hero {
  text-align: center;
  padding: 4rem 2rem;
}

.hero h1 {
  font-size: 3rem;
  margin-bottom: 1rem;
  color: var(--color-heading);
}

.hero p {
  font-size: 1.2rem;
  margin-bottom: 2rem;
  color: var(--color-text);
}

.actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
}

.btn {
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.3s ease;
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
  border: 2px solid var(--color-border-hover);
}

.btn-secondary:hover {
  background-color: var(--color-border-hover);
  color: white;
}
</style>
