using FluentValidation;

namespace Application.UseCases.EditProduct;

public sealed class EditProductValidator
    : AbstractValidator<EditProductCommand>
{
    public EditProductValidator()
    {

        RuleFor(product => product.Price)
            .GreaterThan(0);
        RuleFor(product => product.Name)
            .MaximumLength(100);
        RuleFor(product => product.Description)
            .MaximumLength(500);
        RuleFor(product => product.Category)
            .IsInEnum();
    }
}
