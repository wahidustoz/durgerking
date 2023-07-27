using System.Globalization;
using DurgerKing.Resources;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DurgerKing.Services;

public partial class UpdateHandler
{
    private async Task HandleCallBackQueryAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken cancellationToken = default)
    {
        var task = query.Data switch
        {
            _ when query.Data.Contains("languages")
                => HandleLanguageCallbackAsync(client,query,cancellationToken),
            _ when query.Data == Button.LanguageSettings
                => responseService.SendLanguageSettingsAsync(query.Message.Chat.Id, query.From.Id, cancellationToken).AsTask(),
            _ when query.Data.Contains("update")
                => SendContactRequestAsync(client, query.Message.Chat.Id, cancellationToken),
            _ when query.Data.Contains("addLocation")
                => SendLocationRequestAsync(client, query.Message.Chat.Id, cancellationToken),
            _ => throw new NotImplementedException($"Call back query {query.Data} not supported!")
        };
        await client.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId, cancellationToken);
        await task;
    }

    private async Task SendLocationRequestAsync(ITelegramBotClient client, long chatId, CancellationToken cancellationToken)
    {
        var keyboardLayout = new KeyboardButton[][]
        {
            new KeyboardButton[] { KeyboardButton.WithRequestLocation("Send location 📍")},
        };

        await client.SendTextMessageAsync(
            chatId: chatId,
            text: "Send your location",
            replyMarkup: new ReplyKeyboardMarkup(keyboardLayout) { ResizeKeyboard = true },
            cancellationToken: cancellationToken);
    }

    private static async Task SendContactRequestAsync(ITelegramBotClient client, long chatId, CancellationToken cancellationToken)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Request Contact ☎️", "contact.request"),
            }
        });
        await client.SendTextMessageAsync(
            chatId: chatId,
            text: "Send your contact",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

    private async Task HandleLanguageCallbackAsync(ITelegramBotClient client, CallbackQuery query, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstAsync(u => u.Id == query.From.Id, cancellationToken);
        user.Language = query.Data[(query.Data.IndexOf(".") + 1)..];

        await dbContext.SaveChangesAsync(cancellationToken);
        await responseService.SendLanguageSettingsAsync(query.Message.Chat.Id, query.From.Id, cancellationToken);
    }
} 