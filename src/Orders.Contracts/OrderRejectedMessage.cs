namespace Orders.Contracts;

public class OrderRejectedMessage
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; }
}