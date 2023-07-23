using DurgerKing.Dtos;
using DurgerKing.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace durgerking.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator(IAppDbContext dbContext)
    {
        RuleFor(dto => dto.Name).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(dto => dto.Description).MaximumLength(500);
        RuleFor(dto => dto.Price).GreaterThan(0);
        RuleFor(dto => dto.DiscountPercentage).InclusiveBetween(0, 1);
        RuleFor(dto => dto.CategoryId).GreaterThan(0);

        RuleFor(dto => dto.CategoryId).MustAsync(async (request, ctx, token)
                => await dbContext.Categories.AnyAsync(category => category.Id == request.CategoryId, token))
            .WithMessage("Categegory with CategoryId doesn't exist in the system.");
    }
}