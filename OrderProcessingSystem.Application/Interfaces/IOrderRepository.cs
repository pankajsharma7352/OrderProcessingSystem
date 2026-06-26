using OrderProcessingSystem.Domain.Entities;

namespace OrderProcessingSystem.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task UpdateAsync(Order order);
}
