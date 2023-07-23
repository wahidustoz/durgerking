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
        else if(message.Text=="Contact ☎️")
            await CheckContactAsync(botClient, message,cancellationToken);

        if (message.Contact is not null)
        {
          var user = await dbContext.Users
          .Where(u => u.Id == message.From.Id)
          .Include(u => u.Contact)
          .FirstOrDefaultAsync(cancellationToken);

          var contact = user.Contact;

          contact.UserId = message.From.Id;
          contact.PhoneNumber = message.Contact.PhoneNumber;
          contact.FirstName = message.From.FirstName;
          contact.LastName = message.From.LastName;
          contact.Vcard = message.Contact.Vcard;
         // dbContext.Users.FirstOrDefault(x => x.Id == message.From.Id).Contact = contact;
          dbContext.Contacts.Add(contact);

          await dbContext.SaveChangesAsync();
        }

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

    private   async Task  CheckContactAsync(ITelegramBotClient botClient,Message message, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
        .Where(u => u.Id == message.From.Id)
        .Include(u => u.Contact)
        .FirstOrDefaultAsync(cancellationToken);

        var contact = user.Contact;

        if (contact == null)
        {
             var keyboardLayout = new KeyboardButton[][]
            {
                new KeyboardButton[] { KeyboardButton.WithRequestContact ("Contact ☎️") },
            };
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Send your contact",
                replyMarkup: new ReplyKeyboardMarkup(keyboardLayout) { ResizeKeyboard = true },
                cancellationToken: cancellationToken);
        }
        

        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Update", callbackData: "update"),
            },
        });
        await botClient.SendTextMessageAsync(
                message.Chat.Id,
                contact.ToString(),
                replyMarkup: inlineKeyboard ,
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