using durgerking.Entity;

namespace DurgerKing.Entity;

public class User : IHasTime
{
    public long Id { get; set; }
    public string Fullname { get; set; }
    public string Username { get; set; }
    public string Language { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public virtual Contact Contact { get; set; }
    public virtual List<Location> Locations { get; set; } = new List<Location>();
}