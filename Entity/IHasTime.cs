namespace DurgerKing.Entity;
public interface IHasTimeStamp
{
    public DateTime CreatedAt { get; set; }
    public DateTime  ModifiedAt { get; set; }
}