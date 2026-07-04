using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Data;
using ProductService.Application.Products.Commands;
using BuildingBlocks.CQRS;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// .NET 10 improvements
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/product-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // .NET 10 built-in OpenAPI support
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(5)
    ));

// CQRS MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
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

// .NET 10: Native API Gateway support (YARP)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// .NET 10: Map OpenAPI endpoints
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Auto migrate database
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// .NET 10: Minimal API with Typed Results
app.MapPost("/api/products", async (CreateProductCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return TypedResults.Created($"/api/products/{result.Id}", result);
})
.WithName("CreateProduct")
.WithOpenApi();

app.MapGet("/api/products", async (IMediator mediator) =>
{
    var query = new GetProductsQuery();
    var result = await mediator.Send(query);
    return TypedResults.Ok(result);
})
.WithName("GetProducts")
.WithOpenApi();

app.MapReverseProxy(); // .NET 10 built-in reverse proxy

app.Run();