using FluentValidation;

namespace Application.UseCases.CreateProduct;

public sealed class CreateProductValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(product => product.Price)
            .GreaterThan(0);
        RuleFor(product => product.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(product => product.Description)
            .NotEmpty()
            .MaximumLength(500);
        RuleFor(product => product.Category)
            .NotNull()
            .IsInEnum();
    }
}
