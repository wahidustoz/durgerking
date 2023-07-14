using Telegram.Bot;
using Telegram.Bot.Types;

namespace DurgerKing.Services;

public partial class UpdateHandler
{
    public async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var username = message.From?.Username
            ?? message.From.FirstName;
        logger.LogInformation("Reieved message from {username}", username);

        if(message.Text == "/start" || message.Text == "/help")
            await SendGreetingMessageAsycn(botClient, message, cancellationToken);
    }

    private async Task SendGreetingMessageAsycn(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var username = message.From?.Username
            ?? message.From.FirstName;
        var greeting = $"Greetings, hungry {username}! 🌟🍔 Durgerking is here to make your day scrumptiously delightful. Join us for a burger adventure filled with sizzling flavors, crunchy goodness, and pure happiness. Get ready to take a big bite and experience burger heaven! 🍟😍";
        
        await botClient.SendTextMessageAsync(
            text: greeting,
            chatId: message.Chat.Id,
            cancellationToken: cancellationToken);
    }
}