namespace Orders.Contracts;

public class OrderCreatedEvent
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
}