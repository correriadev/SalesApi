FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/SalesApi/SalesApi.WebApi/SalesApi.WebApi.csproj", "src/SalesApi/SalesApi.WebApi/"]
COPY ["src/SalesApi/SalesApi.Domain/SalesApi.Domain.csproj", "src/SalesApi/SalesApi.Domain/"]
COPY ["src/SalesApi/SalesApi.Infrastructure/SalesApi.Infrastructure.csproj", "src/SalesApi/SalesApi.Infrastructure/"]
COPY ["Directory.Packages.props", "./"]
RUN dotnet restore "src/SalesApi/SalesApi.WebApi/SalesApi.WebApi.csproj"
COPY . .
WORKDIR "/src/src/SalesApi/SalesApi.WebApi"
RUN dotnet build "SalesApi.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SalesApi.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["shared-settings/sharedSettings.json", "/app/sharedSettings.json"]
ENTRYPOINT ["dotnet", "SalesApi.WebApi.dll"] 