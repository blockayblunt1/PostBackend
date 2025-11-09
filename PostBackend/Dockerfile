# syntax=docker/dockerfile:1

# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["PostBackend.csproj", "/src/"]
RUN dotnet restore "/src/PostBackend.csproj"

# Copy everything and publish
COPY . /src
RUN dotnet publish "/src/PostBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# NOTE: For SQL Server connection when running in Docker, pass the connection string via env var:
#   -e ConnectionStrings__DefaultConnection="Server=host.docker.internal,1433;Database=PostDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
# If using a local SQL Server instance, host.docker.internal points to the host.

ENTRYPOINT ["dotnet", "PostBackend.dll"]
