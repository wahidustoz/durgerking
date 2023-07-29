namespace DurgerKing.Services;

public interface IAddressService
{
    ValueTask<string> GetAddressTextAsync(decimal longitute, decimal latitude, CancellationToken cancellationToken);
}
