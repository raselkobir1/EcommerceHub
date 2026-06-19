# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first (layer cache)
COPY ["EcommerceHub.sln", "."]
COPY ["src/EcommerceHub.Shared.Kernel/EcommerceHub.Shared.Kernel.csproj", "src/EcommerceHub.Shared.Kernel/"]
COPY ["src/EcommerceHub.API/EcommerceHub.API.csproj", "src/EcommerceHub.API/"]
COPY ["src/Modules/Auth/EcommerceHub.Modules.Auth/EcommerceHub.Modules.Auth.csproj", "src/Modules/Auth/EcommerceHub.Modules.Auth/"]
COPY ["src/Modules/Catalog/EcommerceHub.Modules.Catalog/EcommerceHub.Modules.Catalog.csproj", "src/Modules/Catalog/EcommerceHub.Modules.Catalog/"]
COPY ["src/Modules/Orders/EcommerceHub.Modules.Orders/EcommerceHub.Modules.Orders.csproj", "src/Modules/Orders/EcommerceHub.Modules.Orders/"]
COPY ["src/Modules/Cart/EcommerceHub.Modules.Cart/EcommerceHub.Modules.Cart.csproj", "src/Modules/Cart/EcommerceHub.Modules.Cart/"]
COPY ["src/Modules/Inventory/EcommerceHub.Modules.Inventory/EcommerceHub.Modules.Inventory.csproj", "src/Modules/Inventory/EcommerceHub.Modules.Inventory/"]
COPY ["src/Modules/Promotions/EcommerceHub.Modules.Promotions/EcommerceHub.Modules.Promotions.csproj", "src/Modules/Promotions/EcommerceHub.Modules.Promotions/"]
COPY ["src/Modules/Customers/EcommerceHub.Modules.Customers/EcommerceHub.Modules.Customers.csproj", "src/Modules/Customers/EcommerceHub.Modules.Customers/"]
COPY ["src/Modules/Suppliers/EcommerceHub.Modules.Suppliers/EcommerceHub.Modules.Suppliers.csproj", "src/Modules/Suppliers/EcommerceHub.Modules.Suppliers/"]
COPY ["src/Modules/Reviews/EcommerceHub.Modules.Reviews/EcommerceHub.Modules.Reviews.csproj", "src/Modules/Reviews/EcommerceHub.Modules.Reviews/"]
COPY ["src/Modules/Payments/EcommerceHub.Modules.Payments/EcommerceHub.Modules.Payments.csproj", "src/Modules/Payments/EcommerceHub.Modules.Payments/"]
COPY ["src/Modules/Notifications/EcommerceHub.Modules.Notifications/EcommerceHub.Modules.Notifications.csproj", "src/Modules/Notifications/EcommerceHub.Modules.Notifications/"]
COPY ["src/Modules/Reports/EcommerceHub.Modules.Reports/EcommerceHub.Modules.Reports.csproj", "src/Modules/Reports/EcommerceHub.Modules.Reports/"]

RUN dotnet restore "src/EcommerceHub.API/EcommerceHub.API.csproj"

COPY . .

WORKDIR "/src/src/EcommerceHub.API"
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EcommerceHub.API.dll"]
