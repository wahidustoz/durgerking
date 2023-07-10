using Microsoft.EntityFrameworkCore;
namespace DurgerKing.Entity.Data;
interface IAppDbContext
{
    public DbSet<User> Users { get; set; }
}