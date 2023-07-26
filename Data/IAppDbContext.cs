using DurgerKing.Entity;
using Microsoft.EntityFrameworkCore;

namespace DurgerKing.Data;

public interface IAppDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<ProductMedium> ProductMedia { get; set; }
    DbSet<Contact> Contacts { get; set; }
    DbSet<Location> Locations { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}