FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY HRM.Resources/*.csproj HRM.Resources/

COPY HRM.Core/*.csproj HRM.Core/
COPY HRM.Core/packages.config HRM.Core/

COPY HRM.MailDeliver/*.csproj HRM.MailDeliver/
COPY HRM.MailDeliver/packages.config HRM.MailDeliver/

COPY HRM.Models/*.csproj HRM.Models/
COPY HRM.Models/packages.config HRM.Models/

COPY HRM.ViewModels/*.csproj HRM.ViewModels/
COPY HRM.ViewModels/packages.config HRM.ViewModels/

COPY HRM.Criteria/*.csproj HRM.Criteria/
COPY HRM.Criteria/packages.config HRM.Criteria/

COPY HRM.Repositories/*.csproj HRM.Repositories/
COPY HRM.Repositories/packages.config HRM.Repositories/

COPY HRM.Services/*.csproj HRM.Services/
COPY HRM.Services/packages.config HRM.Services/

COPY HRM.Letstalk.Core/*.csproj HRM.Letstalk.Core/
COPY HRM.Letstalk.Core/packages.config HRM.Letstalk.Core/

COPY HRM.Controllers/*.csproj HRM.Controllers/
COPY HRM.Controllers/packages.config HRM.Controllers/

COPY HRM.Site/*.csproj HRM.Site/
COPY HRM.Site/packages.config HRM.Site/

# Copy source codes
COPY HRM.Resources/ HRM.Resources/
COPY HRM.Core/ HRM.Core/
COPY HRM.MailDeliver/ HRM.MailDeliver/
COPY HRM.Models/ HRM.Models/
COPY HRM.ViewModels/ HRM.ViewModels/
COPY HRM.Criteria/ HRM.Criteria/
COPY HRM.Repositories/ HRM.Repositories/
COPY HRM.Services/ HRM.Services/
COPY HRM.Letstalk.Core/ HRM.Letstalk.Core/
COPY HRM.Controllers/ HRM.Controllers/
COPY HRM.Site/ HRM.Site/

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
