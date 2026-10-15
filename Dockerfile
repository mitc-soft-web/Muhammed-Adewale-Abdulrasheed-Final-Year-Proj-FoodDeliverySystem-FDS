
# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FoodDeliverySystem/FoodDeliverySystem.csproj FoodDeliverySystem/
RUN dotnet restore FoodDeliverySystem/FoodDeliverySystem.csproj

COPY . .
RUN dotnet publish FoodDeliverySystem/FoodDeliverySystem.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "FoodDeliverySystem.dll"]
