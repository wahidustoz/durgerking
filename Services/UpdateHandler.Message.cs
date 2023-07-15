using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DurgerKing.Services;

public partial class UpdateHandler
{
    public async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var username = message.From?.Username ?? message.From.FirstName;
        logger.LogInformation("Received message from {username}", username);

        if(message.Text == "/start" || message.Text == "/help")
            await SendGreetingMessageAsycn(botClient, message, cancellationToken);
        else if(message.Text == "/settings")
            await SelectSettingsAsync(botClient, message, cancellationToken);
    }

    private async Task SendGreetingMessageAsycn(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var username = message.From?.Username ?? message.From.FirstName;
        var greeting = $"Greetings, hungry {username}! 🌟🍔 Durgerking is here to make your day scrumptiously delightful. Join us for a burger adventure filled with sizzling flavors, crunchy goodness, and pure happiness. Get ready to take a big bite and experience burger heaven! 🍟😍";

        var replyKeyboardMarkup = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new KeyboardButton[] { "Settings ⚙️", "Menu 🍔" },
                new KeyboardButton[] { "Orders 📝" },
            }) { ResizeKeyboard = true };

        await botClient.SendTextMessageAsync(
            text: greeting,
            chatId: message.Chat.Id,
            replyMarkup: replyKeyboardMarkup,
            cancellationToken: cancellationToken);
    }

    private async Task SelectSettingsAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var keyboardLayout = new KeyboardButton[][]
        {
            new KeyboardButton[] { "Language 🎏", "Locations 📌", },
            new KeyboardButton[] { "Contact ☎️" },
        };

        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            "Please select a setting:",
            replyMarkup: new ReplyKeyboardMarkup(keyboardLayout) { ResizeKeyboard = true },
            cancellationToken: cancellationToken);
    }
}