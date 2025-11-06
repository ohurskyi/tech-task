using NodaTime;

namespace Orders.Database;

public class OrderEntity
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public decimal? Amount { get; set; }
    public bool Processed { get; set; }
    public Instant? CreatedAt { get; set; }
}