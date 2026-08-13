using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.Products.Queries;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Products.Queries
{
    public class GetProductsQueryHandler
        : IRequestHandler<GetProductsQuery, IEnumerable<Product>>
    {
        private readonly ProductDbContext _dbContext;
        private readonly IDistributedCache _cache;

        private const string CacheKey = "products:all";

        public GetProductsQueryHandler(
            ProductDbContext dbContext,
            IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<IEnumerable<Product>> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Try Redis first
            var cachedProducts = await _cache.GetStringAsync(
                CacheKey,
                cancellationToken);

            if (!string.IsNullOrEmpty(cachedProducts))
            {
                return JsonSerializer.Deserialize<List<Product>>(
                    cachedProducts) ?? new List<Product>();
            }

            // 2. Cache miss -> get products from SQL Server
            var products = await _dbContext.Products
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // 3. Store products in Redis
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(
                CacheKey,
                JsonSerializer.Serialize(products),
                cacheOptions,
                cancellationToken);

            // 4. Return products
            return products;
        }
    }
}