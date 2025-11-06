FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Orders.Api/Orders.Api.csproj", "src/Orders.Api/"]
COPY ["src/Orders.Contracts/Orders.Contracts.csproj", "src/Orders.Contracts/"]
COPY ["src/Orders.Database/Orders.Database.csproj", "src/Orders.Database/"]
RUN dotnet restore "src/Orders.Api/Orders.Api.csproj"
COPY . .
WORKDIR "/src/src/Orders.Api"
RUN dotnet build "./Orders.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orders.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orders.Api.dll"]
