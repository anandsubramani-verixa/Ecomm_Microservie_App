Write-Host "Installing NuGet packages for .NET 10..." -ForegroundColor Green

# ---------------- Product Service API ----------------
dotnet add src/ProductService/ProductService.API package Microsoft.EntityFrameworkCore.Design -v 10.0.0
dotnet add src/ProductService/ProductService.API package Microsoft.AspNetCore.Authentication.JwtBearer -v 10.0.0
dotnet add src/ProductService/ProductService.API package Swashbuckle.AspNetCore -v 7.2.0
dotnet add src/ProductService/ProductService.API package Serilog.AspNetCore -v 9.0.0
dotnet add src/ProductService/ProductService.API package Serilog.Sinks.Console -v 6.0.0
dotnet add src/ProductService/ProductService.API package Serilog.Sinks.File -v 6.0.0
dotnet add src/ProductService/ProductService.API package Microsoft.Extensions.Caching.StackExchangeRedis -v 10.0.0
dotnet add src/ProductService/ProductService.API package MediatR -v 12.4.1
dotnet add src/ProductService/ProductService.API package Yarp.ReverseProxy -v 2.2.0
dotnet add src/ProductService/ProductService.API package Microsoft.OpenApi -v 2.1.0
dotnet add src/ProductService/ProductService.API package AspNetCore.HealthChecks.SqlServer -v 9.0.0

# ---------------- Product Service Infrastructure ----------------
dotnet add src/ProductService/ProductService.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer -v 10.0.0
dotnet add src/ProductService/ProductService.Infrastructure package Microsoft.EntityFrameworkCore.Tools -v 10.0.0
dotnet add src/ProductService/ProductService.Infrastructure package Polly -v 8.5.0
dotnet add src/ProductService/ProductService.Infrastructure package Azure.Security.KeyVault.Secrets -v 4.7.0
dotnet add src/ProductService/ProductService.Infrastructure package Azure.Identity -v 1.14.2

# ---------------- Order Service API ----------------
dotnet add src/OrderService/OrderService.API package Microsoft.EntityFrameworkCore.Design -v 10.0.0
dotnet add src/OrderService/OrderService.API package Microsoft.AspNetCore.Authentication.JwtBearer -v 10.0.0
dotnet add src/OrderService/OrderService.API package Swashbuckle.AspNetCore -v 7.2.0
dotnet add src/OrderService/OrderService.API package Serilog.AspNetCore -v 9.0.0
dotnet add src/OrderService/OrderService.API package Serilog.Sinks.Console -v 6.0.0
dotnet add src/OrderService/OrderService.API package Serilog.Sinks.File -v 6.0.0
dotnet add src/OrderService/OrderService.API package Microsoft.Extensions.Caching.StackExchangeRedis -v 10.0.0
dotnet add src/OrderService/OrderService.API package MediatR -v 12.4.1
dotnet add src/OrderService/OrderService.API package Yarp.ReverseProxy -v 2.2.0
dotnet add src/OrderService/OrderService.API package Microsoft.OpenApi -v 2.1.0
dotnet add src/OrderService/OrderService.API package AspNetCore.HealthChecks.SqlServer -v 9.0.0


# ---------------- Order Service Infrastructure ----------------
dotnet add src/OrderService/OrderService.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer -v 10.0.0
dotnet add src/OrderService/OrderService.Infrastructure package Microsoft.EntityFrameworkCore.Tools -v 10.0.0
dotnet add src/OrderService/OrderService.Infrastructure package Polly -v 8.5.0
dotnet add src/OrderService/OrderService.Infrastructure package Azure.Security.KeyVault.Secrets -v 4.7.0
dotnet add src/OrderService/OrderService.Infrastructure package Azure.Identity -v 1.14.2
# ---------------- BuildingBlocks.CQRS ----------------
dotnet add src/BuildingBlocks/BuildingBlocks.CQRS package MediatR -v 12.4.1
dotnet add src/BuildingBlocks/BuildingBlocks.CQRS package FluentValidation -v 11.11.0
dotnet add src/BuildingBlocks/BuildingBlocks.CQRS package FluentValidation.DependencyInjectionExtensions -v 11.11.0

Write-Host "Packages installed successfully!" -ForegroundColor Green

# ---------------- Cleanup: remove redundant System.Text.Json refs ----------------
Write-Host "Removing redundant System.Text.Json references (built into .NET 10 shared framework)..." -ForegroundColor Yellow
dotnet remove src/BuildingBlocks/BuildingBlocks.Messaging package System.Text.Json
dotnet remove src/BuildingBlocks/BuildingBlocks.Domain package System.Text.Json

Write-Host "Running solution restore..." -ForegroundColor Green
dotnet restore ECommerceMicroservices.slnx

Write-Host "Building solution..." -ForegroundColor Green
dotnet build ECommerceMicroservices.slnx

Write-Host "Done." -ForegroundColor Green