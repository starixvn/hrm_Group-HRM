FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY **/*.csproj .
COPY **/packages.config .

# Copy the rest of the source code
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

# Restore dependencies
RUN dotnet restore -v n -r linux-x64 HRM.Site/HRM.Site.csproj

# Publish the application
WORKDIR /src/HRM.Site
RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

WORKDIR /app
COPY --from=builder /app .

ENV TZ="America/La_Paz" 
ENV COMPlus_EnableDiagnostics=0

# Don’t run as root
RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

ENTRYPOINT ["dotnet", "HRM.Site.dll"]
