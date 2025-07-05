FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AuthApi/AuthApi.csproj", "AuthApi/"]
RUN dotnet restore "AuthApi/AuthApi.csproj"
COPY . .
WORKDIR "/src/AuthApi"
RUN dotnet build "./AuthApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER $APP_UID

ENTRYPOINT ["dotnet", "AuthApi.dll"]
