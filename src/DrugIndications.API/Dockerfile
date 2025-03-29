# Imagen base de .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar toda la solución
COPY . .

# Restaurar dependencias desde el proyecto principal
RUN dotnet restore src/DrugIndications.API/DrugIndications.API.csproj

# Compilar y publicar
RUN dotnet publish src/DrugIndications.API/DrugIndications.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DrugIndications.API.dll"]
