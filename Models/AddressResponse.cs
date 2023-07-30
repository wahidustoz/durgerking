using System.Text.Json.Serialization;

namespace DurgerKing.Models;

public class AddressResponse
{
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }
}