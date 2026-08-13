namespace OrderService.API.Contracts;

public class CreateOrderRequest
{
    public string CustomerId { get; set; } = string.Empty;

    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}