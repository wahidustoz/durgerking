using DurgerKing.Entity.Data;
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
            await SelectSettingsAsync(botClient, message, cancellationToken);
        else if (message.Text == "Language 🎏")
            await SendSelectLanguageInlineAsync(botClient,message.From.Id,message.Chat.Id,cancellationToken);
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