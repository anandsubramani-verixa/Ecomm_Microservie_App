Write-Host "Installing NuGet packages for .NET 10..." -ForegroundColor Green

# Product Service API
dotnet add src/ProductService/ProductService.API package Microsoft.EntityFrameworkCore.Design -v 9.0.0
dotnet add src/ProductService/ProductService.API package Microsoft.AspNetCore.Authentication.JwtBearer -v 9.0.0
dotnet add src/ProductService/ProductService.API package Swashbuckle.AspNetCore -v 7.2.0
dotnet add src/ProductService/ProductService.API package Serilog.AspNetCore -v 9.0.0
dotnet add src/ProductService/ProductService.API package Microsoft.Extensions.Caching.StackExchangeRedis -v 9.0.0

# Product Service Infrastructure
dotnet add src/ProductService/ProductService.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer -v 9.0.0
dotnet add src/ProductService/ProductService.Infrastructure package Microsoft.EntityFrameworkCore.Tools -v 9.0.0
dotnet add src/ProductService/ProductService.Infrastructure package Polly -v 8.5.0
dotnet add src/ProductService/ProductService.Infrastructure package Azure.Security.KeyVault.Secrets -v 4.7.0
dotnet add src/ProductService/ProductService.Infrastructure package Azure.Identity -v 1.13.0

# Order Service API
dotnet add src/OrderService/OrderService.API package Microsoft.EntityFrameworkCore.Design -v 9.0.0
dotnet add src/OrderService/OrderService.API package Microsoft.AspNetCore.Authentication.JwtBearer -v 9.0.0
dotnet add src/OrderService/OrderService.API package Swashbuckle.AspNetCore -v 7.2.0
dotnet add src/OrderService/OrderService.API package Serilog.AspNetCore -v 9.0.0
dotnet add src/OrderService/OrderService.API package Microsoft.Extensions.Caching.StackExchangeRedis -v 9.0.0

# Order Service Infrastructure
dotnet add src/OrderService/OrderService.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer -v 9.0.0
dotnet add src/OrderService/OrderService.Infrastructure package Microsoft.EntityFrameworkCore.Tools -v 9.0.0
dotnet add src/OrderService/OrderService.Infrastructure package Polly -v 8.5.0
dotnet add src/OrderService/OrderService.Infrastructure package Azure.Security.KeyVault.Secrets -v 4.7.0
dotnet add src/OrderService/OrderService.Infrastructure package Azure.Identity -v 1.13.0

Write-Host "Packages installed successfully!" -ForegroundColor Green