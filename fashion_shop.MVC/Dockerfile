FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5123
ENV ASPNETCORE_URLS=http://+:5123
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY fashion_shop.MVC/fashion_shop.MVC.csproj ./fashion_shop.MVC/
COPY fashion_shop.Core/fashion_shop.Core.csproj ./fashion_shop.Core/
COPY fashion_shop.Infrastructure/fashion_shop.Infrastructure.csproj ./fashion_shop.Infrastructure/

# Restore
RUN dotnet restore ./fashion_shop.MVC/fashion_shop.MVC.csproj

# Copy all project files into container
COPY . .

# Build
WORKDIR /src/fashion_shop.MVC
RUN dotnet build fashion_shop.MVC.csproj -c Release -o /app/build

# ======== Publish ========
FROM build AS publish
RUN dotnet publish fashion_shop.MVC.csproj -c Release -o /app/publish /p:UseAppHost=false

# ======== Final Runtime Image ========
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./fashion_shop.MVC/appsettings.Development.json ./appsettings.Development.json
ENTRYPOINT ["dotnet", "fashion_shop.MVC.dll"]
