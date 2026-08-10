using MediatR;
using ProductService.Domain.Entities;
using System.Collections.Generic;

namespace ProductService.Application.Products.Queries
{
    public class GetProductsQuery : IRequest<IEnumerable<Product>>
    {
    }
}
