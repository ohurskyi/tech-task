using Carter;
using Carter.ModelBinding;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.Filters;
using Swashbuckle.AspNetCore.Filters;

namespace Orders.Api.Features.CreateOrder;

public class CreateOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/orders", [SwaggerRequestExample(typeof(CreateOrderCommand), typeof(CreateOrderCommandExamples))] 
                async Task<Results<Accepted, ValidationProblem>> (HttpRequest request,
                [FromBody] CreateOrderCommand command,
                OrderProcessor processor) =>
            {
                var validationResult = await request.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return TypedResults.ValidationProblem(validationResult.GetValidationProblems());
                }

                var draftId = await processor.Process(command);
                return TypedResults.Accepted($"/api/orders/status/{draftId}");
            })
            .WithTags("Orders")
            .WithName("CreateOrder")
            .WithDescription("Places a new order and publishes it to RabbitMQ via Wolverine");
    }
}