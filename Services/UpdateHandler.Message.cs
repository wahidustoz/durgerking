using Microsoft.EntityFrameworkCore;
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
            await SelectSettingsAsync(botClient, message.Chat.Id, cancellationToken);
        else if (message.Text == "/language")
            await SendSelectLanguageInlineAsync(botClient,message.From.Id,message.Chat.Id,cancellationToken);
    }

    private async Task SendGreetingMessageAsycn(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var username = message.From?.Username ?? message.From.FirstName;
        var greeting = messageLocalizer["greeting-msg", username]; 
        var inlineKeyboard = new InlineKeyboardMarkup(new []
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Settings ⚙️", "settings"),
                InlineKeyboardButton.WithCallbackData("Menu 🍔", "menu") 
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData( "Orders 📝", "orders")
            }
        });

        await botClient.SendTextMessageAsync(
            text: greeting,
            chatId: message.Chat.Id,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

    private static async Task SelectSettingsAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData("Language 🎏", "language"),
                InlineKeyboardButton.WithCallbackData("Locations 📌", "locations")
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData("Contact ☎️", "contact")
            }
        });

        await botClient.SendTextMessageAsync(
            chatId : chatId,
            "Please select a setting:",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }
    
    public async Task SendSelectLanguageInlineAsync(ITelegramBotClient client,long chatId,long userId,CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstAsync(u => u.Id == userId,cancellationToken);
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        { 
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                     text: $"{GetCheckmarkOrEmpty(user.Language, "uz")}O'zbekcha🇺🇿",
                     callbackData : "language.uz"),
                InlineKeyboardButton.WithCallbackData(
                     text: $"{GetCheckmarkOrEmpty(user.Language, "en")}English🇬🇧",
                     callbackData : "language.en"),
                InlineKeyboardButton.WithCallbackData(
                     text: $"{GetCheckmarkOrEmpty(user.Language, "ru")}Русский🇷🇺",
                     callbackData : "language.ru")
          
            }
        });
        
        await client.SendTextMessageAsync(
            chatId : chatId,
            text: "Please Select a language",
            replyMarkup : inlineKeyboard,
            cancellationToken : cancellationToken);
    }

    private static string GetCheckmarkOrEmpty(string userLanguage, string languageCode)
        => string.Equals(userLanguage, languageCode, StringComparison.InvariantCultureIgnoreCase)
        ? "✅"
        :string.Empty;
} 