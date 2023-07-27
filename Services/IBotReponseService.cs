namespace DurgerKing.Services;

public interface IBotResponseService
{
    ValueTask<(long ChatId, long MessageId)> SendGreetingAsync(
        long chatId, 
        CancellationToken cancellationToken = default);

}
