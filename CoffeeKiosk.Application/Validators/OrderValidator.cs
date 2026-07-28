using CoffeeKiosk.Application.DTOs;
using FluentValidation;

namespace CoffeeKiosk.Application.Validators;

public class OrderValidator : AbstractValidator<CreateOrderDto>
{
    public OrderValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0");

            item.RuleFor(i => i.SizeId)
                .GreaterThan(0).WithMessage("SizeId must be greater than 0");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");
        });
    }
}