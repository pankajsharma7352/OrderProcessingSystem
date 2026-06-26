using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Application.DTOs;
using OrderProcessingSystem.Application.Interfaces;
using OrderProcessingSystem.Domain.Entities;

namespace OrderProcessingSystem.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository repository, IMessageBus messageBus, ILogger<OrderService> logger)
    {
        _repository = repository;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerName))
            throw new ArgumentException("Customer name is required.");

        if (dto.Items == null || dto.Items.Count == 0)
            throw new ArgumentException("Order must contain at least one item.");

        var order = new Order
        {
            CustomerName = dto.CustomerName.Trim(),
            CreatedDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            OrderItems = dto.Items.Select(i => new OrderItem
            {
                ProductName = i.ProductName.Trim(),
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };

      
        order.TotalAmount = order.OrderItems.Sum(i => i.Quantity * i.Price);

        var savedOrder = await _repository.CreateAsync(order);
        _logger.LogInformation("Order {OrderId} created with status Pending.", savedOrder.Id);

        await _messageBus.PublishAsync("orders/created", new
        {
            OrderId = savedOrder.Id,
            EventType = "order_created",
            Timestamp = DateTime.UtcNow
        });

        return MapToResponse(savedOrder);
    }

    public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
    {
        var order = await _repository.GetByIdAsync(id);
        return order == null ? null : MapToResponse(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
    {
        var orders = await _repository.GetAllAsync();
        return orders.Select(MapToResponse);
    }

    public async Task ProcessOrderAsync(int orderId)
    {
        var order = await _repository.GetByIdAsync(orderId);

        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found for processing.", orderId);
            return;
        }

        if (order.Status == OrderStatus.Processed)
        {
            _logger.LogInformation("Order {OrderId} is already processed.", orderId);
            return;
        }

        await Task.Delay(9000);

        order.Status = OrderStatus.Processed;
        await _repository.UpdateAsync(order);

        _logger.LogInformation("Order {OrderId} has been processed successfully.", orderId);
    }

    private static OrderResponseDto MapToResponse(Order order) => new()
    {
        Id = order.Id,
        CustomerName = order.CustomerName,
        TotalAmount = order.TotalAmount,
        Status = order.Status.ToString(),
        CreatedDate = order.CreatedDate,
        Items = order.OrderItems.Select(i => new OrderItemResponseDto
        {
            Id = i.Id,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            Price = i.Price
        }).ToList()
    };
}
