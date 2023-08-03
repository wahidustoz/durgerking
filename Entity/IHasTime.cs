namespace durgerking.Entity;

public interface IHasTime
{
    DateTime CreatedAt { get; set; }
    DateTime ModifiedAt { get; set; }
}
