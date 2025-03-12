FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src
# Copy source codes
COPY HRM.Criteria/*.csproj HRM.Criteria/
COPY HRM.Letstalk.Core/*.csproj HRM.Letstalk.Core/
COPY HRM.Models/*.csproj HRM.Models/
COPY HRM.Services/*.csproj HRM.Services/
COPY HRM.Core/*.csproj HRM.Core/
COPY HRM.MailDeliver/*.csproj HRM.MailDeliver/
COPY HRM.Resources/*.csproj HRM.Resources/
COPY HRM.Repositories/*.csproj HRM.Repositories/
COPY HRM.Controllers/*.csproj HRM.Controllers/
COPY HRM.ViewModels/*.csproj HRM.ViewModels/
COPY HRM.Website/*.csproj HRM.Website/
COPY HRM.AutoJobs/*.csproj HRM.AutoJobs/

# Copy the rest of the source code
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

# Restore dependencies
RUN dotnet restore -v n -r linux-x64 NexHR.sln

# Publish the application
WORKDIR /src/HRM.Website
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

ENTRYPOINT ["dotnet", "HRM.Website.dll"]
