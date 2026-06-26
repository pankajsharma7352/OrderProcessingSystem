namespace OrderProcessingSystem.Application.Interfaces;

public interface IMessageBus
{
    Task PublishAsync(string topic, object message);
    void Subscribe(string topic, Func<string, Task> handler);
}
