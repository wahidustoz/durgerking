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
        var route = $"/reverse?lat={latitude.ToString().Replace(",", "."):F5}&lon={longitute.ToString().Replace(",", "."):F5}";
        var address = await this.addressClient.GetFromJsonAsync<AddressResponse>(route);
        
        return $"{address.Address.County}, {address.Address.Road}, {address.Address.HouseNumber}";
    }
}