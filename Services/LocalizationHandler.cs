using System.Reflection;
using DurgerKing.Resources;
using Microsoft.Extensions.Localization;

namespace Durgerking.Services;

public class LocalizationHandler : ILocalizationHandler
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public LocalizationHandler(
        IServiceScopeFactory serviceScopeFactory) 
            => this.serviceScopeFactory = serviceScopeFactory;

    public string GetValue(string key, params string[] arguments)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var types = GetResourceClasses(typeof(Button).Namespace)
            .Select(c => typeof(IStringLocalizer<>).MakeGenericType(c));
        var localizers = types.Select(t => scope.ServiceProvider.GetService(t) as IStringLocalizer);

        foreach(var localizer in localizers)
        {
            var value = localizer.GetString(key, arguments);
            if(value != key)
                return value;
        }

        return key;
    }

    private static Type[] GetResourceClasses(string @namespace)
    {
        Assembly asm = Assembly.GetExecutingAssembly();
        return asm.GetTypes()
            .Where(type => type.Namespace == @namespace)
            .ToArray();
    }
}