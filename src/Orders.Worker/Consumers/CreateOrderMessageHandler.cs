using Microsoft.EntityFrameworkCore;
using NodaTime;
using Orders.Contracts;
using Orders.Database;
using Wolverine.EntityFrameworkCore;

namespace Orders.Worker.Consumers;

public class CreateOrderMessageHandler(
    ILogger<CreateOrderMessageHandler> logger,
    IDbContextOutbox<OrdersDbContext> outbox,
    DomainValidator domainValidator)
{
    public async Task Handle(CreateOrderMessage message)
    {
        logger.LogInformation("Processing order: {ID} for Customer: {CustomerValue}", message.OrderId, message.CustomerId);
        
        await domainValidator.Validate(message);
        
        var order = await outbox.DbContext.Orders.FirstOrDefaultAsync(x => x.Id == message.OrderId);
        
        if (order == null)
        {
            logger.LogWarning("Received unknown Order: {ID}, for Customer: {CustomerId}", message.OrderId, message.CustomerId);
            return;
        }
        
        var createdAt = SystemClock.Instance.GetCurrentInstant();
        order.Amount = message.TotalAmount;
        order.Processed = true;
        order.CreatedAt = createdAt;
        
        var created = new OrderCreatedEvent
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Total = order.Amount.GetValueOrDefault(),
            CreatedAt = createdAt.ToDateTimeUtc()
        };
        await outbox.PublishAsync(created);
        await outbox.SaveChangesAndFlushMessagesAsync();
        logger.LogInformation("Order processed: {ID} for Customer: {CustomerValue} Created At: {Date}", created.Id, created.CustomerId, created.CreatedAt);
    }
}