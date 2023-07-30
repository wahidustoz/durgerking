namespace Durgerking.Services;

public interface ILocalizationHandler
{
    string GetValue(string key, params string[] arguments);
}