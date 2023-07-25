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
        else if(message.Text == "Locations 📌")
            await SendShowAddButtonsAsync(botClient, message, cancellationToken);
        else if(message.Location is not null)
            await UpsertLocationAsync(botClient, message, cancellationToken);
    }

    private async Task UpsertLocationAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Where(u => u.Id == message.From.Id)
            .Include(u => u.Locations)
            .FirstOrDefaultAsync(cancellationToken);
        
        if(user.Locations.Count() != 3)
        {
            var addressText = await addressService.GetAddressTextAsync(
                latitude: message.Location.Latitude,
                longitute: message.Location.Longitude,
                cancellationToken: cancellationToken
            );

            var location = new DurgerKing.Entity.Location
            {
                Latitude = Convert.ToDecimal(message.Location.Latitude),
                Longitute = Convert.ToDecimal(message.Location.Longitude),
                Address = addressText  
            };

            user.Locations.Add(location);

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: addressText,
                cancellationToken: cancellationToken
            );
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SendGreetingMessageAsycn(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
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

    private static async Task SelectSettingsAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
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

    private async Task SendShowAddButtonsAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Where(u => u.Id == message.From.Id)
            .Include(u => u.Locations)
            .FirstOrDefaultAsync(cancellationToken);
        
        var keyboardLayout = user.Locations.Count() < 3 ? 
            new KeyboardButton[][]
            {
                new KeyboardButton[] {"Show locations 👁", KeyboardButton.WithRequestLocation("Add location ➕")},
            }
            :
            new KeyboardButton[][]
            {
                new KeyboardButton[] {"Show locations 👁"},
            };

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Select show or add location",
            replyMarkup: new ReplyKeyboardMarkup(keyboardLayout) { ResizeKeyboard = true },
            cancellationToken: cancellationToken
        );
    }

    private static string GetCheckmarkOrEmpty(string userLanguage, string languageCode)
        => string.Equals(userLanguage, languageCode, StringComparison.InvariantCultureIgnoreCase)
        ? "✅"
        :string.Empty;
}