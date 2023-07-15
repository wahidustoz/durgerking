using Microsoft.EntityFrameworkCore;
namespace DurgerKing.Entity.Data;
public interface IAppDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<Product> Products { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}