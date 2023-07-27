using Telegram.Bot.Types.ReplyMarkups;

namespace DurgerKing.Services;

public class BotResponseService : IBotResponseService
{
    public async ValueTask<(long ChatId, long MessageId)> SendGreetingAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var username = message.From?.Username ?? message.From.FirstName;
        var greeting = messageLocalizer["greeting-msg", username]; 
        var replyKeyboardMarkup = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new KeyboardButton[] { "Settings ⚙️", "Menu 🍔" },
                new KeyboardButton[] { "Orders 📝" }
            }) { ResizeKeyboard = true };

        await botClient.SendTextMessageAsync(
            text: greeting,
            chatId: message.Chat.Id,
            replyMarkup: replyKeyboardMarkup,
            cancellationToken: cancellationToken);
    }
}