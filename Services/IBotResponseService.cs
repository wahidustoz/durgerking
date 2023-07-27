namespace DurgerKing.Services;

public interface IBotResponseService
{
    ValueTask<(long ChatId, long MessageId)> SendGreetingAsync(
        long chatId, 
        long userId, 
        CancellationToken cancellationToken = default);
    ValueTask<(long ChatId, long MessageId)> SendLanguageSettingsAsync(
        long chatId, 
        long userId, 
        CancellationToken cancellationToken = default);
    ValueTask<(long ChatId, long MessageId)> SendMainMenuAsync(
        long chatId,
        CancellationToken cancellationToken = default);
    ValueTask<(long ChatId, long MessageId)> SendSettingsAsync(
        long chatId,
        CancellationToken cancellationToken = default);
}
