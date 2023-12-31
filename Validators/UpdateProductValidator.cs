using DurgerKing.Dtos;
using DurgerKing.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace durgerking.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator(IAppDbContext dbContext)
    {
        RuleFor(dto => dto.Name).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(dto => dto.Description).NotEmpty().MaximumLength(500);
        RuleFor(dto => dto.Price).GreaterThan(0);
        RuleFor(dto => dto.DiscountPercentage).InclusiveBetween(0, 1);
        RuleFor(dto => dto.CategoryId).GreaterThan(0);

        RuleFor(dto => dto.CategoryId).MustAsync(async (request, ctx, token)
                => await dbContext.Categories.AnyAsync(category => category.Id == request.CategoryId, token))
            .WithMessage("{PropertyName} with value {PropertyValue} does not exist.");
    }
}