using Telegram.Bot;
using Telegram.Bot.Types;
namespace DurgerKing.Services;
public partial class UpdateHandler
{
    public Task HandleInlineQueryAsync(ITelegramBotClient botClient, InlineQuery inlineQuery, CancellationToken cancellationToken)
    {
       var username = inlineQuery.From?.Username
            ?? inlineQuery.From.FirstName;
        logger.LogInformation("Reieved CallbackQuery from {username}", username);
        return Task.CompletedTask;
    }
}
