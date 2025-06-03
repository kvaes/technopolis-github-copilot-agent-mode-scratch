# Use the official .NET runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EventsApi.csproj", "."]
RUN dotnet restore "EventsApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "EventsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Azure Functions Core Tools
RUN apt-get update && \
    apt-get install -y curl && \
    curl -sL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs && \
    npm install -g azure-functions-core-tools@4 --unsafe-perm true && \
    apt-get clean

ENTRYPOINT ["func", "start", "--port", "80"]