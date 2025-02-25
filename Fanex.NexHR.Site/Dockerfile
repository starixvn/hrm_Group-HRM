# escape=`
FROM hub.nexdev.net/base/rocketor/buildservice AS builder

ARG DEPLOY_ENV

SHELL ["cmd", "/S", "/C"]

WORKDIR /build

# Copy files *.sln, *.csproj, packages.config to restore nuget packages
COPY *.sln .
COPY Fanex.NexHR.Resources\*.csproj Fanex.NexHR.Resources\

COPY Fanex.NexHR.Core\*.csproj Fanex.NexHR.Core\
COPY Fanex.NexHR.Core\packages.config Fanex.NexHR.Core\

COPY Fanex.NexHR.MailDeliver\*.csproj Fanex.NexHR.MailDeliver\
COPY Fanex.NexHR.MailDeliver\packages.config Fanex.NexHR.MailDeliver\

COPY Fanex.NexHR.Models\*.csproj Fanex.NexHR.Models\
COPY Fanex.NexHR.Models\packages.config Fanex.NexHR.Models\

COPY Fanex.NexHR.ViewModels\*.csproj Fanex.NexHR.ViewModels\
COPY Fanex.NexHR.ViewModels\packages.config Fanex.NexHR.ViewModels\

COPY Fanex.NexHR.Criteria\*.csproj Fanex.NexHR.Criteria\
COPY Fanex.NexHR.Criteria\packages.config Fanex.NexHR.Criteria\

COPY Fanex.NexHR.Repositories\*.csproj Fanex.NexHR.Repositories\
COPY Fanex.NexHR.Repositories\packages.config Fanex.NexHR.Repositories\

COPY Fanex.NexHR.Services\*.csproj Fanex.NexHR.Services\
COPY Fanex.NexHR.Services\packages.config Fanex.NexHR.Services\

COPY HRM.Letstalk.Core\*.csproj HRM.Letstalk.Core\
COPY HRM.Letstalk.Core\packages.config HRM.Letstalk.Core\

COPY Fanex.NexHR.Controllers\*.csproj Fanex.NexHR.Controllers\
COPY Fanex.NexHR.Controllers\packages.config Fanex.NexHR.Controllers\

COPY Fanex.NexHR.Site\*.csproj Fanex.NexHR.Site\
COPY Fanex.NexHR.Site\packages.config Fanex.NexHR.Site\

RUN nuget.exe restore NexHR.sln

# Copy source codes
COPY Fanex.NexHR.Resources\ Fanex.NexHR.Resources\
COPY Fanex.NexHR.Core\ Fanex.NexHR.Core\
COPY Fanex.NexHR.MailDeliver\ Fanex.NexHR.MailDeliver\
COPY Fanex.NexHR.Models\ Fanex.NexHR.Models\
COPY Fanex.NexHR.ViewModels\ Fanex.NexHR.ViewModels\
COPY Fanex.NexHR.Criteria\ Fanex.NexHR.Criteria\
COPY Fanex.NexHR.Repositories\ Fanex.NexHR.Repositories\
COPY Fanex.NexHR.Services\ Fanex.NexHR.Services\
COPY HRM.Letstalk.Core\ HRM.Letstalk.Core\
COPY Fanex.NexHR.Controllers\ Fanex.NexHR.Controllers\
COPY Fanex.NexHR.Site\ Fanex.NexHR.Site\

RUN MSBuild.exe "Fanex.NexHR.Site\Fanex.NexHR.Site.csproj" `
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
