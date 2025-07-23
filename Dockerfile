# Multi-stage Dockerfile for Tony Bot - Optimized for Raspberry Pi (ARM64)
# Use the official .NET 10 preview runtime image for ARM64
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview-noble-chiseled-arm64v8 AS base
USER app
EXPOSE 8080
EXPOSE 8081

# Use SDK image for building - ARM64 compatible
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-noble-arm64v8 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["src/Tony/Tony.csproj", "src/Tony/"]
COPY ["src/Abstractions/Abstractions.csproj", "src/Abstractions/"]
COPY ["src/Models/Models.csproj", "src/Models/"]
COPY ["src/Repositories/Repositories.csproj", "src/Repositories/"]
COPY ["src/Services/Services.csproj", "src/Services/"]

# Restore packages for all projects
RUN dotnet restore "src/Tony/Tony.csproj" --runtime linux-arm64

# Copy all source code
COPY src/ ./src/

# Build the application
WORKDIR "/src/src/Tony"
RUN dotnet build "Tony.csproj" -c $BUILD_CONFIGURATION -o /app/build --runtime linux-arm64 --no-restore

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Tony.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --runtime linux-arm64 --no-restore

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Tony.dll"]
