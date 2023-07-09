namespace DurgerKing.Data;

using DurgerKing.Entity;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext 
{
    public DbSet<User> Users;
    public AppDbContext(DbContextOptions<AppDbContext>options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(x => x.TelegramUserId);

        modelBuilder.Entity<User>()
            .Property(x => x.Fullname)
            .HasMaxLength(120);

        modelBuilder.Entity<User>()
            .Property(x => x.PhoneNumber)
            .HasMaxLength(25);

        modelBuilder.Entity<User>()
            .Property(x => x.Username);

        modelBuilder.Entity<User>()
            .Property(x => x.Language);




        base.OnModelCreating(modelBuilder);
    }
}