using durgerking.Entity;
using DurgerKing.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DurgerKing.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly ILogger<AppDbContext> logger;

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductMedium> ProductMedia { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Location> Locations { get; set; }

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ILogger<AppDbContext> logger)
        : base(options) 
        {
            ChangeTracker.StateChanged += OnStateChanged;
            this.logger = logger;
        }

    private void OnStateChanged(object sender, EntityStateChangedEventArgs e)
    {
        logger.LogInformation(
            "{entity} state changed from {from} to {to}", 
            e.Entry.Entity.GetType().Name, 
            e.OldState,
            e.NewState);

        if(e.Entry.Entity is IHasTime hasTime)
            if(e.NewState == EntityState.Added)
                hasTime.CreatedAt = DateTime.UtcNow;
            else if(e.NewState == EntityState.Modified)
                hasTime.ModifiedAt = DateTime.UtcNow;
    }

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
                new { Id = 1, Name = "Ovqat 🍜" },
                new { Id = 2, Name = "Gazak 🍖" },
                new { Id = 3, Name = "Ichimlik 🍹" },
                new { Id = 4, Name = "Salat 🥗" },
                new { Id = 5, Name = "Set 🍔🍹" });

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
            .HasForeignKey(c => c.CategoryId)
            .IsRequired();

        modelBuilder.Entity<Product>()
            .HasMany(u => u.Items)
            .WithMany();

        modelBuilder.Entity<ProductMedium>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Media)
            .WithOne(m => m.Product)
            .HasForeignKey(m => m.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contact>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Contact)
            .WithOne(c => c.User)
            .HasForeignKey<Contact>(c => c.Id)
            .HasPrincipalKey<User>(u => u.Id);
        
        modelBuilder.Entity<Location>()
            .HasKey(c => c.Id);
        
        modelBuilder.Entity<User>()
            .HasMany(p => p.Locations)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateDates(ChangeTracker.Entries<IHasTime>().Where(e => e.State == EntityState.Modified));
        return await base.SaveChangesAsync(cancellationToken);
    }

     private void UpdateDates(IEnumerable<EntityEntry<IHasTime>> entries)
    {
        if(entries.Any() is false)
            return;

        foreach(var entry in entries)
        {
            entry.Entity.ModifiedAt = DateTime.UtcNow;
        }

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(ChangeTracker.DebugView.LongView);
    }
}