# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8090

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["src/SalesApi/SalesApi.csproj", "src/SalesApi/"]
COPY ["src/SalesApi.Domain/SalesApi.Domain.csproj", "src/SalesApi.Domain/"]
COPY ["src/SalesApi.Infrastructure/SalesApi.Infrastructure.csproj", "src/SalesApi.Infrastructure/"]
RUN dotnet restore "./src/SalesApi/SalesApi.csproj"
COPY . .
WORKDIR "/src/src/SalesApi"
RUN dotnet build "./SalesApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SalesApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8090

ENTRYPOINT ["dotnet", "SalesApi.dll"]