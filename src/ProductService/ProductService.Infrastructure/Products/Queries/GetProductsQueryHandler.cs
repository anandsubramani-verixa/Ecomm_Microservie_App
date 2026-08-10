using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Products.Queries;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProductService.Infrastructure.Products.Queries
{
    public class GetProductsQueryHandler
        : IRequestHandler<GetProductsQuery, IEnumerable<Product>>
    {
        private readonly ProductDbContext _dbContext;

        public GetProductsQueryHandler(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Products.ToListAsync(cancellationToken);
        }
    }
}
