using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Application.Interfaces;
using OrderProcessingSystem.Infrastructure.Messaging;

namespace OrderProcessingSystem.Infrastructure.Workers;

public class OrderProcessingWorker : IHostedService
{
    private readonly IMessageBus _messageBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderProcessingWorker> _logger;

    public OrderProcessingWorker(
        IMessageBus messageBus,
        IServiceProvider serviceProvider,
        ILogger<OrderProcessingWorker> logger)
    {
        _messageBus = messageBus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OrderProcessingWorker started. Subscribing to 'orders/created'.");

        _messageBus.Subscribe("orders/created", async payload =>
        {
            try
            {
                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(payload);
                if (orderEvent == null)
                {
                    _logger.LogWarning("Received null event on 'orders/created'.");
                    return;
                }

              
                _logger.LogInformation("WORKER Picked up OrderId={OrderId} | EventType={EventType}",
                    orderEvent.OrderId, orderEvent.EventType);
                _logger.LogInformation("WORKER Order {OrderId} is now PENDING. Processing will start...", orderEvent.OrderId);
               

      
                using var scope = _serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                await orderService.ProcessOrderAsync(orderEvent.OrderId);

                _logger.LogInformation("WORKER ✔️ Order {OrderId} has been marked PROCESSED.", orderEvent.OrderId);
          
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order event.");
            }
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OrderProcessingWorker stopped.");
        return Task.CompletedTask;
    }
}
