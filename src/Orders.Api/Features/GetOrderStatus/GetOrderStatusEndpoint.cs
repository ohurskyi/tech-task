using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Database;

namespace Orders.Api.Features.GetOrderStatus;

public class GetOrderStatusEndpoint : ICarterModule
{
    public class OrderStatus
    {
        public string CustomerId { get; set; }
        public string Status { get; set; }
    }
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/orders/status/{orderId:guid}", async Task<Results<Ok<OrderStatus>, NotFound<ProblemDetails>>> 
            (Guid orderId, OrdersDbContext dbContext) =>
        {
            var orderStatus = await dbContext.Orders
                .AsNoTracking()
                .Where(x => x.Id == orderId)
                .Select(x => new OrderStatus
                {
                    CustomerId = x.CustomerId,
                    Status = x.Processed ? "Processed" : "Pending"
                })
                .FirstOrDefaultAsync();
            
            if (orderStatus == null)
            {
                return TypedResults.NotFound(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "The order with a specified draft ID was not found",
                    Detail = "Order does not exist anymore"
                });
            }
            return TypedResults.Ok(orderStatus);
        }).WithTags("Orders").WithName("GetOrderStatus").WithDescription("Gets the status of an order");
    }
}