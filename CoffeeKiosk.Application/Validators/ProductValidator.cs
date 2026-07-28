using CoffeeKiosk.Application.DTOs;
using FluentValidation;

namespace CoffeeKiosk.Application.Validators;

public class ProductValidator : AbstractValidator<CreateProductDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be no more than 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be no more than 500 characters");

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage("Base price must be greater than 0");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid category");
    }
}