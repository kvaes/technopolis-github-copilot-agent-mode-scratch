# GitHub Codespaces Setup

This guide explains how to set up and use GitHub Codespaces for end-to-end development of the event management system.

## What is GitHub Codespaces?

GitHub Codespaces provides a complete, configurable dev environment in the cloud. It includes all the tools and dependencies needed to develop, test, and debug the application without local setup.

## Quick Start

1. **Open in Codespaces:**
   - Go to the repository on GitHub
   - Click the green "Code" button
   - Select "Codespaces" tab
   - Click "Create codespace on main"

2. **Wait for setup:**
   - Codespaces will automatically install all dependencies
   - This may take 3-5 minutes on first launch

3. **Start development:**
   - Both backend and frontend will be pre-configured
   - Use the integrated terminal to run commands

## Codespaces Configuration

The repository includes a `.devcontainer` configuration that automatically:

- Installs .NET 8.0 SDK
- Installs Node.js 20 and npm
- Installs Azure Functions Core Tools
- Sets up VS Code extensions
- Configures port forwarding
- Sets up environment variables

## Development Workflow in Codespaces

### Running the Full Stack

1. **Terminal 1 - Start Backend:**
   ```bash
   cd backend/EventsApi
   func start --port 7071
   ```

2. **Terminal 2 - Start Frontend:**
   ```bash
   cd frontend/events-app
   npm run dev -- --host 0.0.0.0 --port 5173
   ```

### Accessing the Applications

Codespaces automatically forwards ports and provides URLs:

- **Frontend**: Click on the "Ports" tab and open the port 5173 URL
- **Backend API**: Click on the "Ports" tab and open the port 7071 URL
- **Both URLs are publicly accessible** (with HTTPS) and will be shown in VS Code

### Environment Variables in Codespaces

The frontend will automatically use the correct backend URL thanks to Codespaces port forwarding:

```bash
# Frontend .env will be automatically configured
VITE_API_BASE_URL=https://[codespace-name]-7071.app.github.dev/api
```

## End-to-End Testing in Codespaces

### 1. Full Application Testing

1. **Start both services** (backend and frontend)
2. **Open the frontend URL** from the Ports tab
3. **Test the complete workflow:**
   - Browse events on the home page
   - Create a new event
   - View event details
   - Register for an event
   - Filter events by date/location

### 2. API Testing

1. **Backend testing with curl:**
   ```bash
   # Get the backend URL from Ports tab
   BACKEND_URL="https://[codespace-name]-7071.app.github.dev"
   
   # Test API endpoints
   curl "$BACKEND_URL/api/events"
   
   # Create an event
   curl -X POST "$BACKEND_URL/api/events" \
     -H "Content-Type: application/json" \
     -d '{
       "name": "Test Event",
       "location": "Virtual",
       "date": "2024-12-25",
       "startTime": "14:00"
     }'
   ```

2. **Frontend API integration:**
   - The frontend automatically connects to the backend
   - All CORS is pre-configured
   - No additional setup needed

### 3. Database Simulation

The current setup uses in-memory storage, so:
- Data persists while the backend is running
- Data is reset when the backend restarts
- Perfect for development and testing

## Advanced Codespaces Features

### Using Multiple Terminals

VS Code in Codespaces supports multiple terminals:

1. **Terminal 1**: Backend development
   ```bash
   cd backend/EventsApi
   func start
   ```

2. **Terminal 2**: Frontend development
   ```bash
   cd frontend/events-app
   npm run dev -- --host 0.0.0.0
   ```

3. **Terminal 3**: General commands
   ```bash
   # Build, test, or run other commands
   ```

### Port Forwarding

Codespaces automatically forwards these ports:
- `5173`: Frontend development server
- `7071`: Azure Functions backend
- `3000`: Alternative frontend port

You can add more ports through:
- VS Code Ports panel
- `.devcontainer/devcontainer.json` configuration

### Pre-configured VS Code Extensions

The following extensions are automatically installed:
- Azure Functions
- C# for Visual Studio Code
- Vetur (Vue.js support)
- TypeScript and JavaScript Language Features
- ESLint
- Prettier

### Environment Persistence

- **Code changes**: Automatically synced to GitHub
- **Dependencies**: Cached between sessions
- **Settings**: Preserved across Codespace rebuilds
- **Data**: Reset on backend restart (in-memory storage)

## Codespaces Devcontainer Configuration

Create `.devcontainer/devcontainer.json`:

```json
{
  "name": "Event Management System",
  "image": "mcr.microsoft.com/devcontainers/universal:2",
  "features": {
    "ghcr.io/devcontainers/features/dotnet:2": {
      "version": "8.0"
    },
    "ghcr.io/devcontainers/features/node:1": {
      "version": "20"
    },
    "ghcr.io/devcontainers/features/azure-cli:1": {}
  },
  "postCreateCommand": "npm install -g azure-functions-core-tools@4 --unsafe-perm true && cd frontend/events-app && npm install && cd ../../backend/EventsApi && dotnet restore",
  "forwardPorts": [5173, 7071],
  "portsAttributes": {
    "5173": {
      "label": "Frontend",
      "protocol": "https"
    },
    "7071": {
      "label": "Backend API",
      "protocol": "https"
    }
  },
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-vscode.vscode-typescript-next",
        "ms-dotnettools.csharp",
        "ms-azuretools.vscode-azurefunctions",
        "vue.volar",
        "esbenp.prettier-vscode",
        "ms-vscode.vscode-eslint"
      ]
    }
  }
}
```

## Collaborative Development

### Multiple Developers

1. **Each developer gets their own Codespace**
2. **Shared codebase** through Git
3. **Individual development environments**
4. **Easy code sharing** through pull requests

### Sharing Your Environment

1. **Make your Codespace public** (in Codespace settings)
2. **Share the URLs** for frontend and backend
3. **Others can test your changes** without setup

## Testing Workflow

### Complete End-to-End Test

1. **Start the stack:**
   ```bash
   # Terminal 1
   cd backend/EventsApi && func start
   
   # Terminal 2
   cd frontend/events-app && npm run dev -- --host 0.0.0.0
   ```

2. **Access frontend** through Ports panel

3. **Test user journey:**
   - Create account → Create event → Register → View events
   - Test filters and search functionality
   - Verify API responses

4. **Check backend logs** in Terminal 1

5. **Verify data persistence** during session

### API Testing with Codespaces

```bash
# Use the auto-generated backend URL
curl "https://[your-codespace]-7071.app.github.dev/api/events" | jq

# Test CORS
curl -H "Origin: https://[your-codespace]-5173.app.github.dev" \
     -H "Access-Control-Request-Method: POST" \
     -H "Access-Control-Request-Headers: Content-Type" \
     -X OPTIONS \
     "https://[your-codespace]-7071.app.github.dev/api/events"
```

## Performance and Limitations

### Codespaces Specs
- **2-core machines**: Good for development
- **4-core machines**: Better for intensive builds
- **8-core machines**: Optimal for large projects

### Storage
- **Up to 32GB** persistent storage
- **Automatic cleanup** after inactivity
- **Regular backups** to GitHub

### Network
- **Fast internet connectivity**
- **Global edge locations**
- **HTTPS everywhere**

## Troubleshooting in Codespaces

### Common Issues

1. **Port not accessible:**
   - Check Ports panel
   - Ensure service is running
   - Verify port forwarding settings

2. **Backend not starting:**
   ```bash
   # Check .NET installation
   dotnet --version
   
   # Reinstall Functions tools
   npm install -g azure-functions-core-tools@4 --unsafe-perm true
   ```

3. **Frontend build errors:**
   ```bash
   # Clear and reinstall dependencies
   cd frontend/events-app
   rm -rf node_modules package-lock.json
   npm install
   ```

4. **CORS issues:**
   - Verify backend CORS configuration
   - Check frontend API base URL
   - Ensure both services are running

### Debugging

1. **Backend debugging:**
   - VS Code debugger with Azure Functions
   - Console logs in terminal
   - API testing with curl

2. **Frontend debugging:**
   - Browser developer tools
   - Vue DevTools extension
   - Console logs and network tab

## Best Practices

### Development
1. **Use separate terminals** for backend and frontend
2. **Keep Codespace running** during active development
3. **Commit changes frequently** to avoid data loss
4. **Use port forwarding** for testing external integrations

### Collaboration
1. **Create feature branches** for new development
2. **Share Codespace URLs** for testing
3. **Document changes** in pull requests
4. **Use consistent environments** through devcontainer

### Resource Management
1. **Stop Codespace** when not in use
2. **Use appropriate machine size** for your work
3. **Clean up unused files** to save storage
4. **Monitor usage** through GitHub settings

## Next Steps

After setting up Codespaces:

1. **Test the complete workflow** end-to-end
2. **Explore the codebase** with full IDE support
3. **Make your first changes** and test them
4. **Share your environment** with team members
5. **Set up automated testing** in the cloud

This setup provides a complete, production-like environment for developing and testing the event management system without any local dependencies.