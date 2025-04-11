# Use the base image with ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8123
EXPOSE 8123

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./fashion_shop.API/fashion_shop.API.csproj ./fashion_shop.API/
RUN dotnet restore ./fashion_shop.API/fashion_shop.API.csproj
COPY . .
WORKDIR /src/fashion_shop.API
RUN dotnet build fashion_shop.API.csproj -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "fashion_shop.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ../fashion_shop.API/appsettings.Development.json ./appsettings.Development.json
ENTRYPOINT ["dotnet", "fashion_shop.API.dll"]