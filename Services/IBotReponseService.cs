namespace DurgerKing.Services;

public interface IBotResponseService
{
    ValueTask<(long ChatId, long MessageId)> SendGreetingAsync(
        string username,
        long chatId, 
        CancellationToken cancellationToken = default);

}
