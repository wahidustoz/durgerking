using Telegram.Bot;
using Telegram.Bot.Types;
namespace DurgerKing.Services;
public partial class UpdateHandler
{
    public Task HandleEditedMessageAsync(ITelegramBotClient botClient, Message editedMessage, CancellationToken cancellationToken)
    {
       var username = editedMessage.From?.Username
            ?? editedMessage.From.FirstName;
        logger.LogInformation("Reieved EditedMessage from {username}", username);
        return Task.CompletedTask;
    }
}
