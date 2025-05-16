#build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["inventario-service/inventario-service.csproj", "inventario-service/"]
RUN dotnet restore "inventario-service/inventario-service.csproj"

COPY . .
WORKDIR "/src/inventario-service"
RUN dotnet publish "inventario-service.csproj" -c Release -o /app/publish

#Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose port 8080 so Docker knows the app is listening on it
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "inventario-service.dll"]