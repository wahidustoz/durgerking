using DurgerKing.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DurgerKing.Data;
public class UserIdentityContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public UserIdentityContext(DbContextOptions<UserIdentityContext> options) : base(options)
    {
    }
}