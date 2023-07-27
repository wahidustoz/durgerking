using System.Globalization;
using DurgerKing.Models;

namespace DurgerKing.Services;

public class AddressService
{
    private readonly HttpClient addressClient;

    public AddressService(IHttpClientFactory httpClientFactory)
    {
        this.addressClient = httpClientFactory.CreateClient("GeoCode");
    }

    public async Task<string> GetAddressTextAsync(double longitute, double latitude, CancellationToken cancellationToken)
    {
        var route = $"/reverse?lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitute.ToString(CultureInfo.InvariantCulture)}";
        var address = await this.addressClient.GetFromJsonAsync<AddressResponse>(route);

        return $"{address.Address.County}, {address.Address.Road}, {address.Address.HouseNumber}";
    }
}