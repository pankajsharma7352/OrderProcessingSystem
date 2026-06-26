using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Application.Interfaces;

namespace OrderProcessingSystem.Infrastructure.Messaging;

public class InMemoryMessageBus : IMessageBus
{
    private readonly ConcurrentDictionary<string, List<Func<string, Task>>> _subscribers = new();
    private readonly ILogger<InMemoryMessageBus> _logger;

    public InMemoryMessageBus(ILogger<InMemoryMessageBus> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(string topic, object message)
    {
        var payload = JsonSerializer.Serialize(message);
        _logger.LogInformation("Publishing to topic '{Topic}': {Payload}", topic, payload);

        if (_subscribers.TryGetValue(topic, out var handlers))
        {
    
            _ = Task.Run(async () =>
            {
                foreach (var handler in handlers)
                {
                    try
                    {
                        await handler(payload);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled error in subscriber for topic '{Topic}'", topic);
                    }
                }
            });
        }
        else
        {
            _logger.LogWarning("No subscribers for topic '{Topic}'", topic);
        }

        return Task.CompletedTask;
    }

    public void Subscribe(string topic, Func<string, Task> handler)
    {
        _subscribers.AddOrUpdate(
            topic,
            _ => new List<Func<string, Task>> { handler },
            (_, existing) => { existing.Add(handler); return existing; }
        );

        _logger.LogInformation("Subscribed to topic '{Topic}'", topic);
    }
}
