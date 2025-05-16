# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["pedidos-service/pedidos-service.csproj", "pedidos-service/"]
RUN dotnet restore "pedidos-service/pedidos-service.csproj"

COPY . .
WORKDIR "/src/pedidos-service"
RUN dotnet publish "pedidos-service.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose port 8080 so Docker knows the app is listening on it
EXPOSE 8080

COPY --from=build /app/publish .

# ENTRYPOINT to run the app
ENTRYPOINT ["dotnet", "pedidos-service.dll"]
