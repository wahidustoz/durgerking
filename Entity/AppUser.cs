using Microsoft.AspNetCore.Identity;

namespace DurgerKing.Entity;

public class AppUser : IdentityUser<Guid>
{
    public string Fullname { get; set; }
    public DateTime Birthdate { get; set; }
}