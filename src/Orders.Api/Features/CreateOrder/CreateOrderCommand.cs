using FluentValidation;

namespace Orders.Api.Features.CreateOrder;

public record CreateOrderCommand(
    string CustomerId,
    List<OrderItemDto> Items,
    decimal TotalAmount
);

public record OrderItemDto(string Sku, int Quantity, decimal UnitPrice);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new OrderItemValidator());
        RuleFor(x => x.TotalAmount).NotEmpty();
    }
}

public class OrderItemValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemValidator()
    {
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThan(0);
    }
}