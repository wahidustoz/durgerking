using DurgerKing.Dtos;
using FluentValidation;

public class CreateProductValidator: AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotNull().MaximumLength(50);
        RuleFor(x => x.Description).NotNull().MinimumLength(10);
        RuleFor(x => x.Price).NotNull();
        RuleFor(x => x.DiscountPercentage).NotNull();
        RuleFor(x => x.IsActive).NotNull();
        RuleFor(x => x.CategoryId).NotNull();

    }
}