using CoffeeKiosk.Application.DTOs;
using FluentValidation;

namespace CoffeeKiosk.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(n => n.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
            .MaximumLength(50).WithMessage("Username must be no more than 50 characters long");
        RuleFor(n => n.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(3).WithMessage("Password must be at least 3 characters long")
            .MaximumLength(50).WithMessage("Password must be no more than 50 characters long");
        RuleFor(n => n.Role)
            .IsInEnum().WithMessage("Invalid role");
    }
}
public class LoginUserValidator : AbstractValidator<LoginDto>
{
    public LoginUserValidator()
    {
        RuleFor(n => n.Username)
            .NotEmpty().WithMessage("Username is required");
        RuleFor(n => n.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}