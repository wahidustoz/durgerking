using DurgerKing.Entity;
using DurgerKing.Entity.Data;
using Microsoft.EntityFrameworkCore;
public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

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
            .HasDefaultValue(new DateTime(2023, 7, 10, 11, 29, 16, 314, DateTimeKind.Utc).AddTicks(6142));

        modelBuilder.Entity<User>()
            .Property(e => e.ModifiedAt)
            .HasDefaultValue(new DateTime(2023, 7, 10, 11, 29, 16, 314, DateTimeKind.Utc).AddTicks(6376));

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

        // Product Entity
        modelBuilder.Entity<Product>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Product>()
            .Property(e => e.Name)
            .HasMaxLength(256)
            .IsRequired();

        modelBuilder.Entity<Product>()
            .Property(e => e.Description)
            .HasMaxLength(1024)
            .IsRequired();

        modelBuilder.Entity<Product>()
            .HasOne(u => u.Category)
            .WithMany()
            .HasForeignKey(c=>c.CategoryId)
            .IsRequired();

         modelBuilder.Entity<Product>()
            .HasMany(u => u.Items)
            .WithMany();

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);
}