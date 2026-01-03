# Base image
FROM cgr.dev/chainguard/aspnet-runtime:latest AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Build dependencies
RUN apt-get update; apt-get install nodejs npm -y; apt-get clean

# Build options
ARG BUILD_CONFIGURATION=Release

# Download npm packages
WORKDIR /src/WebAppTemplate.Frontend/Styles

COPY ["WebAppTemplate.Frontend/Styles/package.json", "package.json"]

RUN npm install

# Restore nuget packages
WORKDIR /src

COPY ["WebAppTemplate.Api/WebAppTemplate.Api.csproj", "WebAppTemplate.Api/"]
COPY ["WebAppTemplate.Frontend/WebAppTemplate.Frontend.csproj", "WebAppTemplate.Frontend/"]
COPY ["WebAppTemplate.Shared/WebAppTemplate.Shared.csproj", "WebAppTemplate.Shared/"]

RUN dotnet restore "WebAppTemplate.Api/WebAppTemplate.Api.csproj"
RUN dotnet restore "WebAppTemplate.Frontend/WebAppTemplate.Frontend.csproj"

# Copy over the whole sources
COPY . .

# Build styles
WORKDIR /src/WebAppTemplate.Frontend/Styles
RUN npm run build

# Build projects
WORKDIR "/src/WebAppTemplate.Api"
RUN dotnet build "./WebAppTemplate.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build-api

WORKDIR "/src/WebAppTemplate.Frontend"
RUN dotnet build "./WebAppTemplate.Frontend.csproj" -c $BUILD_CONFIGURATION -o /app/build-frontend

FROM build AS publish

# Publish options
ARG BUILD_CONFIGURATION=Release

# Publish applications
WORKDIR "/src/WebAppTemplate.Api"
RUN dotnet publish "./WebAppTemplate.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish-api /p:UseAppHost=false

WORKDIR "/src/WebAppTemplate.Frontend"
RUN dotnet publish "./WebAppTemplate.Frontend.csproj" -c $BUILD_CONFIGURATION -o /app/publish-frontend /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish-api .
COPY --from=publish /app/publish-frontend/wwwroot ./wwwroot

ENTRYPOINT ["dotnet", "WebAppTemplate.Api.dll"]