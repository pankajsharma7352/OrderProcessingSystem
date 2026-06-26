namespace OrderProcessingSystem.Infrastructure.Messaging;

public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public string EventType { get; set; } = "order_created";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
