using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.Products.Commands;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Products.Commands
{
    public class CreateProductCommandHandler
        : IRequestHandler<CreateProductCommand, int>
    {
        private readonly ProductDbContext _dbContext;
        private readonly IDistributedCache _cache;

        private const string ProductsCacheKey = "products:all";

        public CreateProductCommandHandler(
            ProductDbContext dbContext,
            IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<int> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price
            };

            // 1. Save product to SQL Server
            _dbContext.Products.Add(product);

            await _dbContext.SaveChangesAsync(cancellationToken);

            // 2. Invalidate product-list cache
            // Next GET /api/products will query SQL
            // and rebuild the Redis cache.
            await _cache.RemoveAsync(
                ProductsCacheKey,
                cancellationToken);

            return product.Id;
        }
    }
}