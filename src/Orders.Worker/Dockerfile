FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Orders.Worker/Orders.Worker.csproj", "src/Orders.Worker/"]
COPY ["src/Orders.Contracts/Orders.Contracts.csproj", "src/Orders.Contracts/"]
COPY ["src/Orders.Database/Orders.Database.csproj", "src/Orders.Database/"]
RUN dotnet restore "src/Orders.Worker/Orders.Worker.csproj"
COPY . .
WORKDIR "/src/src/Orders.Worker"
RUN dotnet build "./Orders.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orders.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orders.Worker.dll"]
