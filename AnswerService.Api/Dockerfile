FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AnswerService.Api/AnswerService.Api.csproj", "AnswerService.Api/"]
COPY ["AnswerService.Application/AnswerService.Application.csproj", "AnswerService.Application/"]
COPY ["AnswerService.Domain/AnswerService.Domain.csproj", "AnswerService.Domain/"]
COPY ["AnswerService.Cache/AnswerService.Cache.csproj", "AnswerService.Cache/"]
COPY ["AnswerService.DAL/AnswerService.DAL.csproj", "AnswerService.DAL/"]
COPY ["AnswerService.Outbox/AnswerService.Outbox.csproj", "AnswerService.Outbox/"]
COPY ["AnswerService.GrpcClient/AnswerService.GrpcClient.csproj", "AnswerService.GrpcClient/"]
COPY ["AnswerService.Messaging/AnswerService.Messaging.csproj", "AnswerService.Messaging/"]
COPY ["AnswerService.BackgroundJobs/AnswerService.BackgroundJobs.csproj", "AnswerService.BackgroundJobs/"]
RUN dotnet restore "AnswerService.Api/AnswerService.Api.csproj"
COPY . .
WORKDIR "/src/AnswerService.Api"
RUN dotnet build "./AnswerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AnswerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnswerService.Api.dll"]
