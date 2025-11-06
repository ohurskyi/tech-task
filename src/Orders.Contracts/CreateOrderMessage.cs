namespace Orders.Contracts;

public class CreateOrderMessage
{
    public Guid OrderId { get; set; }
    public string CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
}

public class OrderItem
{
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}