using NodaTime;
using Orders.Database;

namespace Orders.Api.Services;

public class DbInitializer(IServiceScopeFactory scopeFactory, ILogger<DbInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting database initialization");
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
        await SeedSimpleData(context, cancellationToken);
        logger.LogInformation("Database initialization completed");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
    private static async Task SeedSimpleData(OrdersDbContext context, CancellationToken cancellationToken = default)
    {
        context.Orders.AddRange(
            new OrderEntity
            {
                Id = Guid.NewGuid(),
                CustomerId = "C001",
                Amount = 10m,
                Processed = true,
                CreatedAt = SystemClock.Instance.GetCurrentInstant()
            }
        );
        
        await context.SaveChangesAsync(cancellationToken);
    }

}