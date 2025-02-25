# escape=`
FROM hub.nexdev.net/base/rocketor/buildservice AS builder

ARG DEPLOY_ENV

SHELL ["cmd", "/S", "/C"]

WORKDIR /build

# Copy files *.sln, *.csproj, packages.config to restore nuget packages
COPY *.sln .
COPY HRM.Resources\*.csproj HRM.Resources\

COPY HRM.Core\*.csproj HRM.Core\
COPY HRM.Core\packages.config HRM.Core\

COPY HRM.MailDeliver\*.csproj HRM.MailDeliver\
COPY HRM.MailDeliver\packages.config HRM.MailDeliver\

COPY HRM.Models\*.csproj HRM.Models\
COPY HRM.Models\packages.config HRM.Models\

COPY HRM.ViewModels\*.csproj HRM.ViewModels\
COPY HRM.ViewModels\packages.config HRM.ViewModels\

COPY HRM.Criteria\*.csproj HRM.Criteria\
COPY HRM.Criteria\packages.config HRM.Criteria\

COPY HRM.Repositories\*.csproj HRM.Repositories\
COPY HRM.Repositories\packages.config HRM.Repositories\

COPY HRM.Services\*.csproj HRM.Services\
COPY HRM.Services\packages.config HRM.Services\

COPY HRM.Letstalk.Core\*.csproj HRM.Letstalk.Core\
COPY HRM.Letstalk.Core\packages.config HRM.Letstalk.Core\

COPY HRM.Controllers\*.csproj HRM.Controllers\
COPY HRM.Controllers\packages.config HRM.Controllers\

COPY HRM.Site\*.csproj HRM.Site\
COPY HRM.Site\packages.config HRM.Site\

RUN nuget.exe restore NexHR.sln

# Copy source codes
COPY HRM.Resources\ HRM.Resources\
COPY HRM.Core\ HRM.Core\
COPY HRM.MailDeliver\ HRM.MailDeliver\
COPY HRM.Models\ HRM.Models\
COPY HRM.ViewModels\ HRM.ViewModels\
COPY HRM.Criteria\ HRM.Criteria\
COPY HRM.Repositories\ HRM.Repositories\
COPY HRM.Services\ HRM.Services\
COPY HRM.Letstalk.Core\ HRM.Letstalk.Core\
COPY HRM.Controllers\ HRM.Controllers\
COPY HRM.Site\ HRM.Site\

RUN MSBuild.exe "HRM.Site\HRM.Site.csproj" `
    /t:Build;WebPublish /m `
    /p:DebugType=None `
    /p:Configuration=Release `
    /p:WebPublishMethod=FileSystem `
    /p:TransformWebConfigEnabled=False `
    /p:LocalizedBuild=true `
    /p:AutoParameterizationWebConfigConnectionStrings=False `
    /p:MarkWebConfigAssistFilesAsExclude=false `
    /p:publishUrl="C:\Compiled" `
    /consoleloggerparameters:ErrorsOnly

WORKDIR C:\Compiled

# Transform config
RUN echo "%DEPLOY_ENV%" `
    && xcopy "WebConfigs\Web.%DEPLOY_ENV%.config" "Web.config" /Y

FROM hub.nexdev.net/base/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019 AS base

WORKDIR C:\inetpub\wwwroot

COPY --from=builder C:\Compiled .

RUN dir .\bin\

ENTRYPOINT ["C:\\LogMonitor\\LogMonitor.exe", "C:\\ServiceMonitor.exe", "w3svc"]
