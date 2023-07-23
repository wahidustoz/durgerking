namespace DurgerKing.Entity;

public class Location
{
    public Guid Id { get; set; }
    public decimal Longitute { get; set; }
    public decimal Latitude { get; set; }
    public string Address { get; set; }

    public virtual long OwnerId { get; set; }
    public virtual User User { get; set; }
}