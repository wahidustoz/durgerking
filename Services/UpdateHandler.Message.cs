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
        {
            await responseService.SendGreetingAsync(message.Chat.Id, message.From.Id, cancellationToken);
            await responseService.SendMainMenuAsync(message.Chat.Id, cancellationToken);
        }
        else if(message.Text == "/settings")
            await responseService.SendSettingsAsync(message.Chat.Id, cancellationToken);
        else if (message.Text == "/language")
            await responseService.SendLanguageSettingsAsync(message.Chat.Id, message.From.Id, cancellationToken);
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
}