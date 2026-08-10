using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Data;
using Serilog;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MediatR;
using ProductService.Application.Products.Commands;   // CreateProductCommand
using ProductService.Application.Behaviors;          // ValidationBehavior, LoggingBehavior
using ProductService.Application.Products.Queries;   // GetProductsQuery
using ProductService.Infrastructure.Products.Queries; // GetProductsQueryHandler

var builder = WebApplication.CreateBuilder(args);

// JSON options
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/product-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();   // built-in OpenAPI

// Database
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(5)
    ));

// CQRS MediatR
builder.Services.AddMediatR(cfg =>
{
    // Register Application layer (commands/queries)
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetProductsQuery).Assembly);

    // Register Infrastructure layer (handlers)
    cfg.RegisterServicesFromAssembly(typeof(GetProductsQueryHandler).Assembly);

    // Pipeline behaviors
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

// Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ProductDbContext>();

// Reverse Proxy (YARP)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   // OpenAPI endpoint

    // Auto migrate DB
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Minimal API endpoints
app.MapPost("/api/products", async (CreateProductCommand command, IMediator mediator) =>
{
    var id = await mediator.Send(command); // returns int
    return TypedResults.Created($"/api/products/{id}", new { Id = id });
})
.WithName("CreateProduct");

app.MapGet("/api/products", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetProductsQuery());
    return TypedResults.Ok(result);
})
.WithName("GetProducts");

app.MapReverseProxy(); // reverse proxy

app.Run();
