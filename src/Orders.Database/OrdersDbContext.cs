using Microsoft.EntityFrameworkCore;

namespace Orders.Database;

public class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<OrderEntity> Orders { get; set; }
}