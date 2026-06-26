using OrderProcessingSystem.Application.DTOs;

namespace OrderProcessingSystem.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderResponseDto?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
    Task ProcessOrderAsync(int orderId);
}
