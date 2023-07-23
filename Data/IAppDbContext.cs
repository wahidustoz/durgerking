using DurgerKing.Entity;
using Microsoft.EntityFrameworkCore;

namespace DurgerKing.Data;

public interface IAppDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<ProductMedium> ProductMedia { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}