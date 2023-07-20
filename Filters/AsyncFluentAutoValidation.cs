using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Durgerking.Filters;

public class AsyncFluentAutoValidation : IAsyncActionFilter
{
    public static int OrderLowerThanModelStateInvalidFilter => -2001;
    
    private readonly IServiceProvider _serviceProvider;

    protected AsyncFluentAutoValidation(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            if ((parameter.BindingInfo?.BindingSource == BindingSource.Body || parameter.BindingInfo?.BindingSource == BindingSource.Query)
                && parameter.ParameterType.IsClass)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);
                var validatorObj = _serviceProvider.GetService(validatorType);

                if (validatorObj is not IValidator validatorTyped)
                    continue;

                var subject = context.ActionArguments[parameter.Name];
                
                var result = await validatorTyped.ValidateAsync(new ValidationContext<object>(subject), context.HttpContext.RequestAborted);
                if (!result.IsValid)
                    result.AddToModelState(context.ModelState, null);
            }
        }
    }
}