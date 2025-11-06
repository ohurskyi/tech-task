using Orders.Contracts;
using Orders.Database;
using Wolverine.EntityFrameworkCore;

namespace Orders.Api.Features.CreateOrder;

public class OrderProcessor(IDbContextOutbox<OrdersDbContext> outbox)
{
    public async Task<Guid> Process(CreateOrderCommand command)
    {
        var draft = new OrderEntity
        {
            CustomerId = command.CustomerId,
        };
        outbox.DbContext.Add(draft);
        
        var message = new CreateOrderMessage
        {
            OrderId = draft.Id,
            CustomerId = command.CustomerId,
            Items = command.Items.Select(x => new OrderItem { Sku = x.Sku, Quantity = x.Quantity, Price = x.UnitPrice}).ToList(),
            TotalAmount = command.TotalAmount,
        };
        await outbox.SendAsync(message);
        await outbox.SaveChangesAndFlushMessagesAsync();
        return draft.Id;
    }
}