using DurgerKing.Entity;
using DurgerKing.Entity.Data;
using Microsoft.EntityFrameworkCore;
public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<User>()
            .Property(e => e.Fullname)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(e => e.Username)
            .HasMaxLength(50);

        modelBuilder.Entity<User>()
            .Property(e => e.Language)
            .HasMaxLength(10);

        modelBuilder.Entity<User>()
            .Property(e => e.Phone)
            .HasMaxLength(20);

        modelBuilder.Entity<User>()
            .Property(e => e.CreatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        modelBuilder.Entity<User>()
            .Property(e => e.ModifiedAt)
            .HasDefaultValue(DateTime.UtcNow);

        modelBuilder.Entity<Category>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Category>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(125);

        modelBuilder.Entity<Category>()
            .HasData(
                new { Id = 1, Name = "Food" },
                new { Id = 2, Name = "Snack" },
                new { Id = 3, Name = "Drink" },
                new { Id = 4, Name = "Salad" },
                new { Id = 5, Name = "Set" });

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);
}