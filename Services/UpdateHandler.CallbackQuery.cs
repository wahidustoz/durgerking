using Telegram.Bot;
using Telegram.Bot.Types;
namespace DurgerKing.Services;
public partial class UpdateHandler
{
    public Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
       var username = callbackQuery.From?.Username
            ?? callbackQuery.From.FirstName;
        logger.LogInformation("Reieved CallbackQuery from {username}", username);
        return Task.CompletedTask;
    }
}
