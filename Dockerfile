# 1. Fase de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia todo el contenido de la carpeta Tienda.API al contenedor
COPY Tienda.API/ ./ 
RUN dotnet publish -c Release -o out

# 2. Fase de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Tienda.API.dll"]
