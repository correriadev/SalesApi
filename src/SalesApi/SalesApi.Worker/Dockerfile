FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8091

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/SalesApi/SalesApi.Worker/SalesApi.Worker.csproj", "src/SalesApi/SalesApi.Worker/"]
COPY ["src/SalesApi/SalesApi.Domain/SalesApi.Domain.csproj", "src/SalesApi/SalesApi.Domain/"]
COPY ["src/SalesApi/SalesApi.Infrastructure/SalesApi.Infrastructure.csproj", "src/SalesApi/SalesApi.Infrastructure/"]
COPY ["Directory.Packages.props", "./"]
RUN dotnet restore "src/SalesApi/SalesApi.Worker/SalesApi.Worker.csproj"
COPY . .
WORKDIR "/src/src/SalesApi/SalesApi.Worker"
RUN dotnet build "SalesApi.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SalesApi.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["shared-settings/sharedSettings.json", "/app/sharedSettings.json"]
ENTRYPOINT ["dotnet", "SalesApi.Worker.dll"] 