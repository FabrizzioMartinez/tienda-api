# Usa la imagen oficial de .NET para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia los archivos y compila
COPY . ./
RUN dotnet publish -c Release -o out

# Usa la imagen de ejecución para el resultado final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expone el puerto y ejecuta
EXPOSE 8080
ENTRYPOINT ["dotnet", "Tienda.API.dll"]