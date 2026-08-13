using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.API.Contracts;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _context;

    public OrdersController(OrderDbContext context)
    {
        _context = context;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ToListAsync();

        var response = orders.Select(MapToResponse);

        return Ok(response);
    }

    // GET: api/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(order));
    }

    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerId))
        {
            return BadRequest("CustomerId is required.");
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            return BadRequest("Order must contain at least one item.");
        }

        if (request.Items.Any(i => i.Quantity <= 0))
        {
            return BadRequest("Order item quantity must be greater than zero.");
        }

        if (request.Items.Any(i => i.UnitPrice < 0))
        {
            return BadRequest("Order item price cannot be negative.");
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow,
            Items = request.Items.Select(item => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        // Calculate total on the server
        order.TotalAmount = order.Items.Sum(
            item => item.Quantity * item.UnitPrice);

        // Set OrderId for every OrderItem
        foreach (var item in order.Items)
        {
            item.OrderId = order.Id;
        }

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        var response = MapToResponse(order);

        return CreatedAtAction(
            nameof(GetOrder),
            new { id = order.Id },
            response);
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,

            Items = order.Items.Select(item => new OrderItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }
}