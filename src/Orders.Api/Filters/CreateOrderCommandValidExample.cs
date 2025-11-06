using Orders.Api.Features.CreateOrder;
using Swashbuckle.AspNetCore.Filters;

namespace Orders.Api.Filters;

public class CreateOrderCommandExamples : IMultipleExamplesProvider<CreateOrderCommand>
{
    public IEnumerable<SwaggerExample<CreateOrderCommand>> GetExamples()
    {
        yield return SwaggerExample.Create("Valid Order (should be accepted and processed)", new CreateOrderCommand(
            CustomerId: "C001",
            Items:
            [
                new OrderItemDto("ITEM-1", 2, 50.0m),
                new OrderItemDto("ITEM-2", 1, 25.0m)
            ],
            TotalAmount: 125.0m
        ));

        yield return SwaggerExample.Create("Invalid Order (should fail simple validation on API)", new CreateOrderCommand(
            CustomerId: "",
            Items: [],
            TotalAmount: 100.0m
        ));
        
        yield return SwaggerExample.Create("Invalid Order (should fail domain validation and move to error queue)", new CreateOrderCommand(
            CustomerId: "CUST-5678",
            Items: [ new OrderItemDto("ITEM-3", 1, 100.0m)],
            TotalAmount: 100.0m
        ));
    }
}