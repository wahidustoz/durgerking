namespace DurgerKing.Services;
public interface IHasTimeStamp
{
    public DateTime CreatedAt { get; set; }
    public DateTime  ModifiedAt { get; set; }
}