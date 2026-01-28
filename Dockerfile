# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/RestaurantBooking.Api/RestaurantBooking.Api.csproj
RUN dotnet publish src/RestaurantBooking.Api/RestaurantBooking.Api.csproj -c Release -o /app/publish

# Estágio de Execução (Final)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RestaurantBooking.Api.dll"]
