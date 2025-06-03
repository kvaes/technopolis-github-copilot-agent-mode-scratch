# Data Contract

This directory contains the data models and contracts that ensure consistency between the frontend and backend applications.

## Models

### Event
Represents an event with the following properties:
- `id`: Unique identifier (string)
- `name`: Event name (string)
- `location`: Event location (string)  
- `date`: Event date (ISO 8601 date string)
- `startTime`: Event start time (string in HH:mm format)
- `createdAt`: Creation timestamp (ISO 8601 datetime string)
- `updatedAt`: Last update timestamp (ISO 8601 datetime string)

### Registration
Represents an event registration with the following properties:
- `id`: Unique identifier (string)
- `eventId`: Associated event ID (string)
- `name`: Registrant name (string)
- `email`: Registrant email (string)
- `pronouns`: Registrant pronouns (string, optional)
- `optInCommunication`: Whether user opted in for further communication (boolean)
- `registeredAt`: Registration timestamp (ISO 8601 datetime string)

## Usage

These models should be implemented consistently across:
- Backend C# classes/DTOs
- Frontend TypeScript interfaces
- API request/response schemas
- Database schemas

## JSON Examples

### Event
```json
{
  "id": "event-123",
  "name": "Tech Conference 2024",
  "location": "Brussels, Belgium",
  "date": "2024-07-15",
  "startTime": "09:00",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-20T14:45:00Z"
}
```

### Registration
```json
{
  "id": "reg-456",
  "eventId": "event-123",
  "name": "John Doe",
  "email": "john.doe@example.com",
  "pronouns": "he/him",
  "optInCommunication": true,
  "registeredAt": "2024-02-01T09:15:00Z"
}
```