using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Net.Mime;
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
        else if(message.Text == "Contact ☎️")
            await CheckContactAsync(botClient, message, cancellationToken);
        else if(message.Contact is not null)
            await UpsertContactAsync(botClient, message, cancellationToken);
        else if(message.Text == "Locations 📌")
            await SendShowAddButtonsAsync(botClient, message, cancellationToken);
        else if(message.Location is not null)
        {
            await UpsertLocationAsync(botClient, message, cancellationToken);
            await SendShowAddButtonsAsync(botClient, message, cancellationToken);
        }
        else if (message.Text == "Menu 🍔")
            await SendCategoryButtonAsync(botClient, message, cancellationToken);
    }
    private async Task SendCategoryButtonAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var categories = await dbContext.Categories
            .ToArrayAsync();

        List<List<InlineKeyboardButton>> buttons = new();
        int count = 0;
        foreach(var Item in categories)
        {
            if(count++ % 3 ==0)
            {
                buttons.Add(new());
            }
            buttons[buttons.Count - 1].Add(InlineKeyboardButton.WithCallbackData(Item.Name, Item.Id.ToString()));
        } 
         var inlineKeyboard = new InlineKeyboardMarkup(buttons); 
         await botClient.SendTextMessageAsync(
            text: "Menudagi barcha taomlar",
            chatId: message.Chat.Id,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);   
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
                Address = addressText,
                IsActive = true
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

    private async Task UpsertContactAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Where(u => u.Id == message.From.Id)
            .Include(u => u.Contact)
            .FirstOrDefaultAsync(cancellationToken);
        
        user.Contact = new DurgerKing.Entity.Contact
        {
            PhoneNumber = message.Contact.PhoneNumber,
            FirstName = message.From.FirstName,
            LastName = message.From.LastName,
            Vcard = message.Contact.Vcard,
        };
        await dbContext.SaveChangesAsync(cancellationToken);
        await SendContactInfoAsync(botClient, message, user.Contact, cancellationToken);
    }

    private async Task SendGreetingMessageAsycn(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var username = message.From?.Username ?? message.From.FirstName;
        var greeting = messageLocalizer["greeting-msg", username]; 
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Settings ⚙️", "settings"),
                InlineKeyboardButton.WithCallbackData("Menu 🍔", "menu")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Orders 📝", "orders")
            }
        });

        await botClient.SendTextMessageAsync(
            text: greeting,
            chatId: message.Chat.Id,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

    private static async Task SelectSettingsAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Language 🎏", "settings.language"),
                InlineKeyboardButton.WithCallbackData("Locations 📌", "settings.locations"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Contact ☎️", "settings.contact"),
            },
        });

        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            "Please select a setting:",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

    private async Task CheckContactAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Where(u => u.Id == message.From.Id)
            .Include(u => u.Contact)
            .FirstOrDefaultAsync(cancellationToken);

        var contact = user.Contact;

        if(contact == null)
            await SendContactRequestAsync(botClient, message.Chat.Id, cancellationToken);
        else
            await SendContactInfoAsync(botClient, message, contact, cancellationToken);
    }

    private static async Task SendContactInfoAsync(ITelegramBotClient botClient, Message message, Entity.Contact contact, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Update", callbackData: "contact-update"),
            },
        });
        var contactText = $"{contact.FirstName} {contact.LastName},PhoneNumber: {contact.PhoneNumber}";
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: contactText,
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

    private async Task SendShowAddButtonsAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Where(u => u.Id == message.From.Id)
            .Include(u => u.Locations)
            .FirstOrDefaultAsync(cancellationToken);
        
        InlineKeyboardMarkup keyboardLayout = user.Locations.Count() < 3 ? 
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Show locations 👁", callbackData: "showLocations"),
                InlineKeyboardButton.WithCallbackData(text: "Add location ➕", callbackData: "addLocation")
            }
            :
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(text: "Show locations 👁", callbackData: "showLocations")
            };

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Select show or add location",
            replyMarkup: keyboardLayout,
            cancellationToken: cancellationToken
        );
    }

    private static string GetCheckmarkOrEmpty(string userLanguage, string languageCode)
        => string.Equals(userLanguage, languageCode, StringComparison.InvariantCultureIgnoreCase)
        ? "✅"
        :string.Empty;
}