# Technopolis GitHub Copilot Agent Mode Scratch

A full-stack application for event management built with VueJS frontend and C# Azure Functions backend.

## Repository Structure

- `.github/workflows`: GitHub Actions workflows for CI/CD
- `backend/`: C# Azure Functions backend API
- `frontend/`: VueJS frontend application
- `datacontract/`: Data models and contracts shared between frontend and backend
- `docs/`: Documentation

## Quick Start

### Prerequisites

- Node.js 18+ and npm
- .NET 8.0 SDK
- Azure Functions Core Tools
- Docker (for containerization)

### Local Development

See [docs/local-development.md](docs/local-development.md) for detailed setup instructions.

### GitHub Codespaces

See [docs/codespaces-setup.md](docs/codespaces-setup.md) for end-to-end development in Codespaces.

## Features

### Backend APIs
- Event CRUD operations (Create, Read, Update, Delete)
- Event filtering by date and location
- Built-in CORS handling
- Error handling and validation

### Frontend
- Event listing with filtering capabilities
- Event details view
- Event registration functionality
- Responsive design

## Data Model

Events include:
- Name
- Location  
- Date
- Start time

Registration includes:
- Name
- Email
- Pronouns
- Communication opt-in

## License

MIT License - see [LICENSE](LICENSE) file for details.