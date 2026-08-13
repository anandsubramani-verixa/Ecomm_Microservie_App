namespace OrderService.API.Contracts;

public class OrderItemResponse
{
    public Guid Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}