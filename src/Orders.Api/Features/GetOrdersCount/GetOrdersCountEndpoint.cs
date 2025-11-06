using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Database;

namespace Orders.Api.Features.GetOrdersCount;

public class GetOrdersCountEndpoint : ICarterModule
{
    public class OrdersCount
    {
        public int Count { get; set; }
        public string Status { get; set; }
    }
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/orders", async Task<Results<Ok<OrdersCount>, BadRequest<string>>>
                ([FromQuery] string status, OrdersDbContext dbContext) =>
            {
                var query = dbContext.Orders.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(status))
                {
                    switch (status.ToLowerInvariant())
                    {
                        case "pending":
                            query = query.Where(x => !x.Processed);
                            break;
                        case "processed":
                            query = query.Where(x => x.Processed);
                            break;
                        default:
                            return TypedResults.BadRequest("Status must be 'pending' or 'processed'");
                    }
                }

                var count = await query.CountAsync();

                return TypedResults.Ok(new OrdersCount
                {
                    Count = count,
                    Status = status ?? "all"
                });
            })
            .WithTags("Orders")
            .WithName("GetOrdersCount")
            .WithDescription("Returns the number of orders by status query: pending, processed, or all if empty (default).");
    }
}