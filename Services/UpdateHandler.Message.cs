using DurgerKing.Exceptions;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
        else if(message.Text == "/locations")
            await responseService.SendLocationKeyboardAsync(message.Chat.Id, message.From.Id, cancellationToken);
        else if(message.Type is MessageType.Location && message.Location is not null)
            await HandleLocationAsync(message, cancellationToken);
        else if(message.Text == "/contact")
            await responseService.SendContactAsync(message.Chat.Id, message.From.Id, cancellationToken);
        else if(message.Contact is not null)
            await HandleContactAsync(message, cancellationToken);
        else if(message.Text == "/pagination")
            await responseService.SendProductPaginationAsync(message.Chat.Id, cancellationToken);
    }

    private async Task HandleContactAsync(Message message, CancellationToken cancellationToken)
    {
        var user = await userService.UpsertContactAsync(
            userId: message.From.Id,
            phone: message.Contact.PhoneNumber,
            firstname: message.Contact.FirstName,
            lastname: message.Contact.LastName,
            vcard: message.Contact.Vcard,
            cancellationToken: cancellationToken);

        await responseService.SendContactAsync(message.Chat.Id, user.Id, cancellationToken);
    }

    private async Task HandleLocationAsync(Message message, CancellationToken cancellationToken)
    {
        try
        {
            await userService.AddLocationAsync(
                userId: message.From.Id,
                latitude: Convert.ToDecimal(message.Location.Latitude),
                longitude: Convert.ToDecimal(message.Location.Longitude),
                cancellationToken: cancellationToken);

            await responseService.SendLocationsAsync(message.Chat.Id, message.From.Id, cancellationToken);
        }
        catch(MaxLocationsExceededException ex)
        {
            logger.LogInformation(ex, "User {userId} exceeded max locations.", message.From.Id);
            await responseService.SendLocationExceededErrorAsync(message.Chat.Id, cancellationToken);
        }
    }
}