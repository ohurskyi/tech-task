using System.Collections.Concurrent;

namespace Orders.Worker.Consumers;

public class CustomerApiService
{
    private static readonly ConcurrentDictionary<string, string> Customers = new()
    {
        ["C001"] = "Customer1",
        ["C002"] = "Customer2",
        ["C003"] = "Customer4",
        ["C004"] = "Customer5",
        ["C005"] = "Customer5"
    };
    
    public Task<string> GetCustomerName(string customerId)
    {
        return Task.FromResult(Customers.GetValueOrDefault(customerId, string.Empty));
    }
}