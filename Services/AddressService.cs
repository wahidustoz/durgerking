using System.Globalization;
using DurgerKing.Models;

namespace DurgerKing.Services;

public class AddressService : IAddressService
{
    private readonly HttpClient client;

    public AddressService(HttpClient client)
        => this.client = client;

    public async ValueTask<string> GetAddressTextAsync(decimal longitute, decimal latitude, CancellationToken cancellationToken)
    {
        var route = "/reverse"
        + $"?lat={latitude.ToString(CultureInfo.InvariantCulture)}"
        + $"&lon={longitute.ToString(CultureInfo.InvariantCulture)}";

        var address = await client.GetFromJsonAsync<AddressResponse>(route, cancellationToken);
        return address.DisplayName;
    }
}