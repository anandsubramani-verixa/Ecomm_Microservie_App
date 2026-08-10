using MediatR;

namespace ProductService.Application.Products.Commands
{
    public class CreateProductCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }   // ✅ add this
        public decimal Price { get; set; }
    }

}
