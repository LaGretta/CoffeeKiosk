using CoffeeKiosk.Application.DTOs;
using FluentValidation;

namespace CoffeeKiosk.Application.Validators;

public class SizeValidator : AbstractValidator<CreateSizeDto>
{
    public SizeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(20).WithMessage("Name must be no more than 20 characters");

        RuleFor(x => x.PriceModifier)
            .GreaterThanOrEqualTo(0).WithMessage("Price modifier cannot be negative");
    }
}