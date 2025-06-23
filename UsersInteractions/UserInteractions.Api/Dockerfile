FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UsersInteractions/UserInteractions.Api/UserInteractions.Api.csproj", "UsersInteractions/UserInteractions.Api/"]
COPY ["UsersInteractions/UsersInteractions.Application/UsersInteractions.Application.csproj", "UsersInteractions/UsersInteractions.Application/"]
COPY ["UsersInteractions/UsersInteractions.Domain/UsersInteractions.Domain.csproj", "UsersInteractions/UsersInteractions.Domain/"]
COPY ["UsersInteractions/UsersInteractions.Infrastructure/UsersInteractions.Infrastructure.csproj", "UsersInteractions/UsersInteractions.Infrastructure/"]
RUN dotnet restore "UsersInteractions/UserInteractions.Api/UserInteractions.Api.csproj"
COPY . .
WORKDIR "/src/UsersInteractions/UserInteractions.Api"
RUN dotnet build "./UserInteractions.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserInteractions.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserInteractions.Api.dll"]
