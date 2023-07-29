using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DurgerKing.Extensions;

public static class TelegramExtensions
{
    public static User ExtractUser(this Update update)
        => update.Type switch
        {
            UpdateType.Message => update.Message.From,
            UpdateType.EditedMessage => update.EditedMessage.From,
            UpdateType.CallbackQuery => update.CallbackQuery.From,
            UpdateType.InlineQuery => update.InlineQuery.From,
            _ => throw new Exception("We dont supportas update type {update.Type} yet")
        };
}