using System.Text.Json.Serialization;

namespace DurgerKing.Models;

public class AddressResponse
{
    [JsonPropertyName("place_id")]
    public int PlaceId { get; set; }

    [JsonPropertyName("lat")]
    public string Lat { get; set; }

    [JsonPropertyName("lon")]
    public string Lon { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("address")]
    public CurrentAddress Address { get; set; }
}