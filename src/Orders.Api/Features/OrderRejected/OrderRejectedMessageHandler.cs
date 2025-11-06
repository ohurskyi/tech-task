using Orders.Contracts;

namespace Orders.Api.Features.OrderRejected;

public class OrderRejectedMessageHandler(ILogger<OrderRejectedMessageHandler> logger)
{
    public Task Handle(OrderRejectedMessage message)
    {
        logger.LogWarning("Order was rejected: {ID}, Reason: {Detail}. Send notification to UI", message.OrderId, message.Reason);
        return Task.CompletedTask;
    }
}