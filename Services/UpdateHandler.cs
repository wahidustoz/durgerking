using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace DurgerKing.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> logger;

    public UpdateHandler(
        ILogger<UpdateHandler> logger)
    {
        this.logger = logger;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Polling error happened.");
        return Task.CompletedTask;
    }

    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var UpdateType = update.Type;
        var UserID = update.Message?.From.Id;

        logger.LogInformation("Type message {updateType} from user Id {userId}",UpdateType,UserID);
        return Task.CompletedTask;
    }
}