using Microsoft.EntityFrameworkCore;
namespace DurgerKing.Entity.Data;
public interface IAppDbContext
{
    public DbSet<User> Users { get; set; }

    Task <int> SaveChangesAsync (CancellationToken cancellationToken = default);
}