# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy solution and restore dependencies (layer caching)
COPY *.sln .
COPY src/WuzApiClient/*.csproj ./src/WuzApiClient/
COPY src/WuzApiClient.RabbitMq/*.csproj ./src/WuzApiClient.RabbitMq/
COPY src/WuzApiClient.Results/*.csproj ./src/WuzApiClient.Results/
COPY tools/WuzApiClient.EventDashboard/*.csproj ./tools/WuzApiClient.EventDashboard/
RUN dotnet restore tools/WuzApiClient.EventDashboard/WuzApiClient.EventDashboard.csproj

# Copy everything and build
COPY src/. ./src/
COPY tools/. ./tools/
WORKDIR /source/tools/WuzApiClient.EventDashboard
RUN dotnet publish -c Release -o /app --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser
USER appuser

COPY --from=build /app ./

# Expose port
EXPOSE 8080

ENTRYPOINT ["dotnet", "WuzApiClient.EventDashboard.dll"]
