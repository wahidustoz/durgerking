using DurgerKing.Data;
using DurgerKing.Entity;
using DurgerKing.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DurgerKing.Services;

public class UserService : IUserService
{
    private const int MAX_ALLOWED_LOCATIONS = 3;
    private readonly ILogger<UserService> logger;
    private readonly IAppDbContext dbContext;
    private readonly IAddressService addressService;

    public UserService(
        ILogger<UserService> logger,
        IAppDbContext dbContext,
        IAddressService addressService)
    {
        this.logger = logger;
        this.dbContext = dbContext;
        this.addressService = addressService;
    }

    public async Task<User> AddLocationAsync(
        long userId, 
        decimal latitude, 
        decimal longitude,
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserWithLocationsOrDefaultAsync(userId, cancellationToken);
        if(user?.Locations?.Count >= MAX_ALLOWED_LOCATIONS)
            throw new MaxLocationsExceededException(MAX_ALLOWED_LOCATIONS);
        
        user.Locations.Add(new Location
        {
            Latitude = latitude,
            Longitute = longitude,
            IsActive = true,
            Address = await addressService.GetAddressTextAsync(
                longitute: longitude, 
                latitude: latitude, 
                cancellationToken: cancellationToken)
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    public Task<User> GetUserOrDefaultAsync(long userId, CancellationToken cancellationToken = default)
        => dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public Task<User> GetUserWithLocationsOrDefaultAsync(long userId, CancellationToken cancellationToken = default)
        => dbContext.Users
        .Include(u => u.Locations.Where(l => l.IsActive))
        .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public async Task<User> RemoveLocationAsync(long userId, Guid locationId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserWithLocationsOrDefaultAsync(userId, cancellationToken);
        var location = user.Locations.FirstOrDefault(l => l.Id == locationId);
        if(location is not null)
            location.IsActive = false;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateLanguageAsync(long userId, string language, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        user.Language = language;
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<User> UpsertUserAsync(long userId, string fullname, string username, string language, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            user = new User
            {
                Id = userId,
                Fullname = fullname,
                Username = username,
                Language = language,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            dbContext.Users.Add(user);
            logger.LogTrace("New user with ID {id} added.", userId);
        }
        else
        {
            user.Fullname = fullname;
            user.Username = username;
            user.ModifiedAt = DateTime.UtcNow;
            logger.LogTrace("user with ID {id} updated.", userId);
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }
}