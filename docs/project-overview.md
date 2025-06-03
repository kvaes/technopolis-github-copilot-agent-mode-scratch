# Project Documentation

This documentation provides an overview of the Event Management System architecture, components, and development guidelines.

## Architecture Overview

The Event Management System is a full-stack application built with modern web technologies:

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   Backend       │    │   Data Layer    │
│   (VueJS)       │────│   (Azure        │────│   (In-Memory)   │
│                 │    │   Functions)    │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Technology Stack

#### Frontend
- **Framework**: Vue.js 3 with Composition API
- **Language**: TypeScript
- **State Management**: Pinia
- **Routing**: Vue Router
- **Build Tool**: Vite
- **Styling**: CSS3 with CSS Variables

#### Backend
- **Platform**: Azure Functions (.NET 8.0)
- **Language**: C#
- **Architecture**: Serverless Functions
- **Runtime**: .NET Isolated Worker
- **Storage**: In-Memory (for development)

#### DevOps
- **CI/CD**: GitHub Actions
- **Containerization**: Docker
- **Dependency Management**: Dependabot
- **Development Environment**: GitHub Codespaces

## Project Structure

```
/
├── backend/
│   └── EventsApi/
│       ├── Functions/           # Azure Function endpoints
│       ├── Models/              # Data models and DTOs
│       ├── Services/            # Business logic services
│       ├── Program.cs           # Application entry point
│       ├── host.json           # Functions host configuration
│       └── local.settings.json # Local development settings
├── frontend/
│   └── events-app/
│       ├── src/
│       │   ├── components/      # Reusable Vue components
│       │   ├── views/          # Page components
│       │   ├── stores/         # Pinia stores
│       │   ├── services/       # API service layer
│       │   ├── types/          # TypeScript interfaces
│       │   ├── router/         # Vue Router configuration
│       │   └── main.ts         # Application entry point
│       ├── public/             # Static assets
│       └── package.json        # Dependencies and scripts
├── datacontract/              # Shared data models
├── docs/                      # Documentation
├── .github/
│   ├── workflows/             # CI/CD workflows
│   ├── dependabot.yml        # Dependency updates
│   └── SECURITY.md           # Security policy
└── .devcontainer/            # Codespaces configuration
```

## Data Models

### Event
The core entity representing an event:

```typescript
interface Event {
  id: string;
  name: string;
  location: string;
  date: string; // ISO 8601 date
  startTime: string; // HH:mm format
  createdAt: string;
  updatedAt: string;
}
```

### Registration
Represents a user registration for an event:

```typescript
interface Registration {
  id: string;
  eventId: string;
  name: string;
  email: string;
  pronouns?: string;
  optInCommunication: boolean;
  registeredAt: string;
}
```

## API Design

### RESTful Endpoints

#### Events
- `GET /api/events` - List events with optional filtering
- `GET /api/events/{id}` - Get specific event
- `POST /api/events` - Create new event
- `PUT /api/events/{id}` - Update event
- `DELETE /api/events/{id}` - Delete event

#### Registrations
- `POST /api/registrations` - Register for event
- `GET /api/events/{eventId}/registrations` - Get event registrations

### Request/Response Format

All API responses follow this structure:

```json
{
  "success": true,
  "data": { /* response data */ },
  "message": "Optional success message",
  "error": null
}
```

Error responses:

```json
{
  "success": false,
  "data": null,
  "message": null,
  "error": "Error description"
}
```

## Frontend Architecture

### Component Structure

```
src/
├── App.vue                 # Root component
├── main.ts                # Application bootstrap
├── components/            # Reusable components
├── views/                 # Page-level components
│   ├── HomeView.vue       # Landing page
│   ├── EventsView.vue     # Events listing
│   ├── EventDetailView.vue # Event details & registration
│   └── CreateEventView.vue # Event creation form
├── stores/
│   └── events.ts          # Event state management
├── services/
│   └── api.ts             # API communication layer
└── types/
    └── index.ts           # TypeScript interfaces
```

### State Management

Uses Pinia for centralized state management:

- **Events Store**: Manages event data, loading states, and API calls
- **Reactive State**: Automatic UI updates when data changes
- **Error Handling**: Centralized error state management

### Routing

Vue Router handles navigation:

- `/` - Home page
- `/events` - Events listing
- `/events/create` - Create event form
- `/events/:id` - Event details and registration

## Backend Architecture

### Azure Functions Structure

```
EventsApi/
├── Functions/
│   ├── EventFunctions.cs      # Event CRUD operations
│   └── RegistrationFunctions.cs # Registration operations
├── Services/
│   ├── IEventService.cs       # Event service interface
│   ├── IRegistrationService.cs # Registration service interface
│   └── EventService.cs        # In-memory implementations
└── Models/
    └── DataModels.cs          # C# data models
```

### Dependency Injection

Services are registered in `Program.cs`:

```csharp
services.AddSingleton<IEventService, InMemoryEventService>();
services.AddSingleton<IRegistrationService, InMemoryRegistrationService>();
```

### CORS Configuration

CORS is handled directly in the Functions code:

```csharp
response.Headers.Add("Access-Control-Allow-Origin", "*");
response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
```

## Security Considerations

### Current Implementation
- Input validation on all endpoints
- CORS configuration for cross-origin requests
- Error handling without information disclosure
- Basic form validation in frontend

### Production Recommendations
- Implement authentication and authorization
- Add rate limiting to prevent abuse
- Use HTTPS everywhere
- Store secrets in Azure Key Vault
- Implement audit logging
- Add input sanitization
- Set up monitoring and alerting

## Development Workflow

### Local Development
1. Start backend: `cd backend/EventsApi && func start`
2. Start frontend: `cd frontend/events-app && npm run dev`
3. Access at http://localhost:5173

### GitHub Codespaces
1. Open repository in Codespaces
2. Both services start automatically
3. Access through forwarded ports

### Continuous Integration
- **Backend**: Build, test, and containerize on changes
- **Frontend**: Type check, build, and containerize on changes
- **Dependencies**: Automated updates via Dependabot

## Testing Strategy

### Current State
- Build verification for both projects
- Type checking for TypeScript
- Basic validation testing

### Recommended Additions
- Unit tests for business logic
- Integration tests for API endpoints
- End-to-end tests for user workflows
- Performance testing
- Security testing

## Deployment

### Containerization
Both projects include Dockerfiles for containerized deployment:

- **Backend**: Multi-stage build with Azure Functions runtime
- **Frontend**: Nginx-based serving with optimized build

### Cloud Deployment Options
- **Azure**: Functions + Static Web Apps
- **AWS**: Lambda + S3/CloudFront
- **Google Cloud**: Cloud Functions + Cloud Storage
- **Kubernetes**: Container orchestration

## Monitoring and Logging

### Development
- Console logging in both frontend and backend
- Browser developer tools for frontend debugging
- Azure Functions runtime logs for backend

### Production Recommendations
- Application Insights for Azure deployments
- Structured logging with correlation IDs
- Error tracking and alerting
- Performance monitoring
- User analytics

## Contributing Guidelines

### Code Style
- **C#**: Follow .NET conventions
- **TypeScript/Vue**: Use ESLint and Prettier
- **Naming**: PascalCase for C#, camelCase for TypeScript
- **Comments**: Document public APIs and complex logic

### Pull Request Process
1. Create feature branch
2. Make changes with tests
3. Update documentation
4. Submit pull request
5. Code review and approval
6. Merge to main

### Git Workflow
- **main**: Production-ready code
- **feature/***: New features
- **bugfix/***: Bug fixes
- **hotfix/***: Critical fixes

## Performance Considerations

### Frontend
- Code splitting with Vue Router
- Lazy loading of components
- Efficient state management
- Optimized bundle size

### Backend
- Stateless functions for scalability
- Efficient data serialization
- Connection pooling (when using databases)
- Caching strategies

## Scalability

### Current Limitations
- In-memory storage (single instance)
- No persistent data layer
- Basic error handling

### Scaling Recommendations
- Add persistent database (Azure SQL, CosmosDB)
- Implement caching layer (Redis)
- Add load balancing
- Implement horizontal scaling
- Add message queues for async processing

## Future Enhancements

### Planned Features
- User authentication system
- Event categories and tags
- Email notifications
- Calendar integration
- Advanced search and filtering
- Event analytics and reporting

### Technical Improvements
- Database integration
- Real-time updates with SignalR
- Mobile application
- Offline support
- Advanced caching
- Microservices architecture

## Resources

### Documentation Links
- [Local Development Setup](local-development.md)
- [GitHub Codespaces Setup](codespaces-setup.md)
- [Security Policy](../.github/SECURITY.md)

### External Resources
- [Vue.js Documentation](https://vuejs.org/)
- [Azure Functions Documentation](https://docs.microsoft.com/en-us/azure/azure-functions/)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)

## Support

For questions, issues, or contributions:
- Create GitHub issues for bugs and feature requests
- Submit pull requests for improvements
- Review documentation for setup and usage
- Follow security policy for vulnerability reports