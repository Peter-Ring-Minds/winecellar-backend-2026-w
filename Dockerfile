# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy everything into /src
COPY . .

# Remove directories with literal backslashes in their names (Windows build artifacts)
RUN find . -depth -name '*\\*' -exec rm -rf {} + 2>/dev/null || true

# Restore using explicit project path
RUN dotnet restore src/Api/Api.csproj

# Publish
RUN dotnet publish src/Api/Api.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Api.dll"]
