namespace OrderService.API.Contracts;

public class OrderResponse
{
    public Guid Id { get; set; }

    public string CustomerId { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public List<OrderItemResponse> Items { get; set; } = new();
}