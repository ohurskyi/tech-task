using Orders.Contracts;

namespace Orders.Worker.Consumers;

public class DomainValidator(CustomerApiService customerApiService)
{
    public async Task Validate(CreateOrderMessage message)
    {
        var customerName = await customerApiService.GetCustomerName(message.CustomerId);
        if (string.IsNullOrEmpty(customerName))
        {
            throw new DomainValidationException($"Unknown Customer: {message.CustomerId}");
        }
        
        var sum = message.Items.Sum(x => x.Price * x.Quantity);
        if (sum != message.TotalAmount)
        {
            throw new DomainValidationException($"Amount mismatch for Order {message.OrderId}: expected {sum}, got {message.TotalAmount}");
        }
    }
}

public class DomainValidationException(string message) : Exception(message);