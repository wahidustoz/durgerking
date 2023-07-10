using DurgerKing.Entity;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public AppDbContext(DbContextOptions options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(e => e.TelegramUserId);
        
        modelBuilder.Entity<User>()
            .Property(e => e.Fullname)
            .HasMaxLength(100)
            .IsRequired();
        
        modelBuilder.Entity<User>()
            .Property(e => e.Username)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(e => e.Language)
            .HasMaxLength(10)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(e => e.Phone)
            .HasMaxLength(20)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}