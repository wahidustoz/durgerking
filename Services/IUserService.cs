using DurgerKing.Entity;

namespace DurgerKing.Services;

public interface IUserService
{
    Task<User> UpsertUserAsync(
        long userId, 
        string fullname,
        string username,
        string language, 
        CancellationToken cancellationToken = default);
    
    Task<User> UpdateLanguageAsync(
        long userId, 
        string language,
        CancellationToken cancellationToken = default);

    Task<User> GetUserOrDefaultAsync(long userId, CancellationToken cancellationToken = default);
    Task<User> GetUserWithLocationsOrDefaultAsync(long userId, CancellationToken cancellationToken = default);
    Task<User> AddLocationAsync(
        long userId, 
        decimal latitude, 
        decimal longitude, 
        CancellationToken cancellationToken = default);
    Task<User> RemoveLocationAsync(
        long userId, 
        Guid locationId,
        CancellationToken cancellationToken = default);
}
