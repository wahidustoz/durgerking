using Durgerking.Dtos;
using DurgerKing.Data;
using FluentValidation;

namespace DurgerKing.Validators;

public class CreateSetValidator : AbstractValidator<CreateSetDto>
{
    public CreateSetValidator(IAppDbContext context)
    {
        RuleFor(dto => dto.ItemIds)
            .NotNull()
            .WithMessage("itemids should not be empty");
        RuleFor(dto => dto.ItemIds)
            .NotEmpty()
            .WithMessage("itemids should not be empty.");
        RuleFor(dto => dto.ItemIds)
            .Must(ids => ids != null && ids.Distinct().Count() == ids.Count())
            .WithMessage("itemids should not have repeated guides");
    }
}