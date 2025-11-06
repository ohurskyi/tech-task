using Orders.Contracts;

namespace Orders.Api.Features.OrderCreated;

public class OrderCreatedMessageHandler(ILogger<OrderCreatedMessageHandler> logger)
{
    public Task Handle(OrderCreatedEvent message)
    {
        logger.LogInformation("Order successfully created: {ID}. Send notification to UI", message.Id);
        return Task.CompletedTask;
    }
}