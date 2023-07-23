namespace DurgerKing.Entity;

public class Contact 
{
    public long? UserId { get; set; }
    public string PhoneNumber { get; set; }
    public string  FirstName { get; set; }
    public string LastName { get; set; }
    public string? Vcard { get; set; }
    public virtual User User { get; set; }
}