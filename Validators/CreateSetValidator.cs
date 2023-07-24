using Durgerking.Dtos;
using DurgerKing.Data;
using FluentValidation;

namespace DurgerKing.Validators;

public class CreateSetValidator : AbstractValidator<CreateSetDto>
{
    public CreateSetValidator(IAppDbContext context)
    {
        RuleFor(dto => dto.itemIds)
            .NotNull()
            .WithMessage("itemids should not be empty");

        RuleFor(dto => dto.itemIds)
            .NotEmpty()
            .WithMessage("itemids should not be empty.");

        RuleFor(dto => dto.itemIds)
            .Must(ids => ids != null && ids.Distinct().Count() == ids.Count())
            .WithMessage("itemids should not have repeated guides");
    }
}