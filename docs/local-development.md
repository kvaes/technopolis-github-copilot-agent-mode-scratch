# Local Development Setup

This guide will help you set up the event management system for local development.

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software
- **Node.js 18+** and **npm** - for the frontend
- **.NET 8.0 SDK** - for the backend
- **Azure Functions Core Tools v4** - for running Azure Functions locally
- **Git** - for version control
- **Docker** (optional) - for containerized development

### Installation Commands

#### Windows (using Chocolatey)
```powershell
choco install nodejs dotnet-8.0-sdk azure-functions-core-tools git docker-desktop
```

#### macOS (using Homebrew)
```bash
brew install node dotnet azure-functions-core-tools git docker
```

#### Ubuntu/Debian
```bash
# Node.js
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt-get install -y nodejs

# .NET 8.0
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0

# Azure Functions Core Tools
npm install -g azure-functions-core-tools@4 --unsafe-perm true

# Git and Docker
sudo apt-get install -y git docker.io
```

## Project Structure

```
├── backend/
│   └── EventsApi/          # C# Azure Functions API
├── frontend/
│   └── events-app/         # VueJS frontend application
├── datacontract/           # Shared data models
├── docs/                   # Documentation
└── .github/                # GitHub workflows and configurations
```

## Backend Setup (C# Azure Functions)

1. **Navigate to the backend directory:**
   ```bash
   cd backend/EventsApi
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

4. **Configure local settings:**
   
   The `local.settings.json` file should already be configured for local development:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
     },
     "Host": {
       "CORS": "*"
     }
   }
   ```

5. **Start the Azure Functions locally:**
   ```bash
   func start
   ```

   The API will be available at `http://localhost:7071`

### Backend API Endpoints

Once running, the following endpoints will be available:

- `GET /api/events` - Get all events (with optional filtering)
- `GET /api/events/{id}` - Get a specific event
- `POST /api/events` - Create a new event
- `PUT /api/events/{id}` - Update an event
- `DELETE /api/events/{id}` - Delete an event
- `POST /api/registrations` - Register for an event
- `GET /api/events/{eventId}/registrations` - Get registrations for an event

## Frontend Setup (VueJS)

1. **Navigate to the frontend directory:**
   ```bash
   cd frontend/events-app
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Configure environment variables:**
   
   Create or verify the `.env` file:
   ```bash
   VITE_API_BASE_URL=http://localhost:7071/api
   ```

4. **Start the development server:**
   ```bash
   npm run dev
   ```

   The frontend will be available at `http://localhost:5173`

### Frontend Development Commands

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run type-check` - Type checking
- `npm run lint` - Lint code (if configured)

## Running Both Applications

To run the full stack locally:

1. **Terminal 1 - Backend:**
   ```bash
   cd backend/EventsApi
   func start
   ```

2. **Terminal 2 - Frontend:**
   ```bash
   cd frontend/events-app
   npm run dev
   ```

3. **Access the application:**
   - Frontend: http://localhost:5173
   - Backend API: http://localhost:7071
   - API Documentation: http://localhost:7071/api (functions list)

## Development Workflow

### Making Changes

1. **Backend Changes:**
   - Edit C# files in `backend/EventsApi/`
   - The Functions runtime will automatically reload on changes
   - Check terminal for any compilation errors

2. **Frontend Changes:**
   - Edit Vue files in `frontend/events-app/src/`
   - Hot Module Replacement (HMR) will update the browser automatically
   - TypeScript errors will show in the terminal and browser

### Testing Changes

1. **Backend Testing:**
   ```bash
   cd backend/EventsApi
   dotnet test  # Run unit tests (when available)
   ```

2. **Frontend Testing:**
   ```bash
   cd frontend/events-app
   npm run type-check  # TypeScript checking
   npm test           # Run unit tests (when available)
   ```

### Building for Production

1. **Backend:**
   ```bash
   cd backend/EventsApi
   dotnet publish -c Release
   ```

2. **Frontend:**
   ```bash
   cd frontend/events-app
   npm run build
   ```

## Troubleshooting

### Common Issues

1. **Azure Functions not starting:**
   - Ensure Azure Functions Core Tools v4 is installed
   - Check that port 7071 is not in use
   - Verify .NET 8.0 SDK is installed

2. **Frontend not loading:**
   - Ensure Node.js 18+ is installed
   - Check that port 5173 is not in use
   - Verify npm dependencies are installed

3. **CORS errors:**
   - Ensure backend is running on http://localhost:7071
   - Check the CORS configuration in `local.settings.json`
   - Verify the frontend's API base URL is correct

4. **TypeScript errors:**
   - Run `npm run type-check` to see detailed errors
   - Ensure all dependencies are installed
   - Check that data contract types are consistent

### Ports

| Service | Default Port | URL |
|---------|-------------|------|
| Backend API | 7071 | http://localhost:7071 |
| Frontend | 5173 | http://localhost:5173 |

### Environment Variables

#### Backend
- `AzureWebJobsStorage`: Storage connection (uses development storage locally)
- `FUNCTIONS_WORKER_RUNTIME`: Set to "dotnet-isolated"

#### Frontend
- `VITE_API_BASE_URL`: Backend API URL (http://localhost:7071/api)

## Next Steps

Once you have the local environment running:

1. Create some test events through the frontend
2. Test the registration functionality
3. Explore the API endpoints with a tool like Postman
4. Review the code structure and make your first changes
5. Set up GitHub Codespaces for cloud development (see [codespaces-setup.md](codespaces-setup.md))

## Additional Resources

- [VueJS Documentation](https://vuejs.org/)
- [Azure Functions Documentation](https://docs.microsoft.com/en-us/azure/azure-functions/)
- [.NET 8.0 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Pinia State Management](https://pinia.vuejs.org/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)