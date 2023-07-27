using System.Text.Json.Serialization;

namespace DurgerKing.Models;

public class CurrentAddress
{
    [JsonPropertyName("house_number")]
    public string HouseNumber { get; set; }

    [JsonPropertyName("road")]
    public string Road { get; set; }

    [JsonPropertyName("quarter")]
    public string Quarter { get; set; }

    [JsonPropertyName("county")]
    public string County { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }
}