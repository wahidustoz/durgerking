using DurgerKing.Entity;

namespace DurgerKing.Dtos;
public class GetLocationDto
{
    public GetLocationDto(Location location)
    {
        Id = location.Id;
        Longitute = location.Longitute;
        Latitude = location.Latitude;
        Address = location.Address;
    }

    public Guid Id { get; set; }
    public decimal Longitute { get; set; }
    public decimal Latitude { get; set; }
    public string Address { get; set; }
}