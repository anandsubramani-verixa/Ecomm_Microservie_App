using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Data;
using Serilog;
using System.Text.Json;
using MediatR;
using ProductService.Application.Products.Commands;
using ProductService.Application.Behaviors;
using ProductService.Application.Products.Queries;
using ProductService.Infrastructure.Products.Queries;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// JSON
// ---------------------------------------------------------

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// ---------------------------------------------------------
// CORS
// ---------------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ---------------------------------------------------------
// Serilog
// ---------------------------------------------------------

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/product-service-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ---------------------------------------------------------
// Controllers / OpenAPI
// ---------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ---------------------------------------------------------
// Database
// ---------------------------------------------------------

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(5);
        }));

// ---------------------------------------------------------
// CQRS / MediatR
// ---------------------------------------------------------

builder.Services.AddMediatR(cfg =>
{
    // Application commands
    cfg.RegisterServicesFromAssembly(
        typeof(CreateProductCommand).Assembly);

    // Application queries
    cfg.RegisterServicesFromAssembly(
        typeof(GetProductsQuery).Assembly);

    // Infrastructure handlers
    cfg.RegisterServicesFromAssembly(
        typeof(GetProductsQueryHandler).Assembly);

    // Pipeline behaviors
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

// ---------------------------------------------------------
// Redis Distributed Cache
// ---------------------------------------------------------

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("Redis");

    options.InstanceName = "ProductService:";
});

// ---------------------------------------------------------
// Health Checks
// ---------------------------------------------------------

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ProductDbContext>();

// ---------------------------------------------------------
// YARP Reverse Proxy
// ---------------------------------------------------------

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy"));

// ---------------------------------------------------------
// Build application
// ---------------------------------------------------------

var app = builder.Build();

// ---------------------------------------------------------
// Development
// ---------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Auto migrate database
    await using var scope = app.Services.CreateAsyncScope();

    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<ProductDbContext>();

    await dbContext.Database.MigrateAsync();
}

// ---------------------------------------------------------
// Middleware
// ---------------------------------------------------------

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// ---------------------------------------------------------
// Product endpoints
// ---------------------------------------------------------

app.MapPost(
    "/api/products",
    async (
        CreateProductCommand command,
        IMediator mediator) =>
    {
        var id = await mediator.Send(command);

        return TypedResults.Created(
            $"/api/products/{id}",
            new { Id = id });
    })
    .WithName("CreateProduct");

app.MapGet(
    "/api/products",
    async (IMediator mediator) =>
    {
        var result =
            await mediator.Send(new GetProductsQuery());

        return TypedResults.Ok(result);
    })
    .WithName("GetProducts");

// ---------------------------------------------------------
// Reverse Proxy
// ---------------------------------------------------------

app.MapReverseProxy();

app.Run();