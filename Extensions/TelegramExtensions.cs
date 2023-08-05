using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
    
    public static InlineKeyboardMarkup CreateInlinePagination(
        this ITelegramBotClient _, 
        IEnumerable<string> source,
        string queryData = "",
        InlinePaginationOptions options = default)
    {
        if (source.Count() < 2)
            throw new InvalidOperationException("Cannot create pagination with less than 2 items.");

        var buttons = new List<InlineKeyboardButton>();
        var callbackData = new CallbackData(queryData);

        // Fix data from source
        if (CanHavePreviousButton(source, options, callbackData))
        {
            buttons.Add(InlineKeyboardButton.WithCallbackData(
                text: options.PreviousButtonText,
                callbackData: $"Name={options.Name}&Index={callbackData.Index - 1}&Data={callbackData.Data}"));
        }

        if (CanHaveNextButton(source, options, callbackData))
        {
            buttons.Add(InlineKeyboardButton.WithCallbackData(
                text: options.NextButtonText,
                callbackData: $"Name={options.Name}&Index={callbackData.Index + 1}&Data={callbackData.Data}"));
        }

        if(CanHavePreviousButton(source, options, callbackData) && CanHaveNextButton(source, options, callbackData))
        {

            buttons.Insert(1, InlineKeyboardButton.WithCallbackData(
                text: $"{callbackData.Index + 1}",
                callbackData: $"Name={options.Name}&Index={callbackData.Index+1}&Data={callbackData.Data}"));
            buttons.Insert(1, InlineKeyboardButton.WithCallbackData(
                text: options.CurrentButtonText,
                callbackData: $"Name={options.Name}&Index={callbackData.Index}&Data={callbackData.Data}"));
            buttons.Insert(1, InlineKeyboardButton.WithCallbackData(
                text: $"{callbackData.Index - 1}",
                callbackData: $"Name={options.Name}&Index={callbackData.Index-1}&Data={callbackData.Data}"));

            return new InlineKeyboardMarkup(buttons);
        }

        if(CanHaveNextButton(source, options, callbackData))
        {
            var start = options.MaxButtonsPerRow - 1;
            var end = 1;
            for(int i = start; i >= end; i--)
            {
                buttons.Insert(0, InlineKeyboardButton.WithCallbackData(
                    text: i == callbackData.Index ? options.CurrentButtonText : $"{i}",
                    callbackData: $"Name={options.Name}&Index={i}&Data={callbackData.Data}"));
            }

            return new InlineKeyboardMarkup(buttons);
        }

        if(CanHavePreviousButton(source, options, callbackData))
        {
            var start = source.Count();
            var end = source.Count() - (options.MaxButtonsPerRow - 1);
            for(int i = start; i > end; i--)
            {
                buttons.Insert(1, InlineKeyboardButton.WithCallbackData(
                    text: i == callbackData.Index ? options.CurrentButtonText : $"{i}",
                    callbackData: $"Name={options.Name}&Index={i}&Data={callbackData.Data}"));
            }

            return new InlineKeyboardMarkup(buttons);
        }

        buttons.AddRange(source
            .Select((s, i) => InlineKeyboardButton.WithCallbackData(
                text: (i + 1) == callbackData.Index ? options.CurrentButtonText : $"{i+1}",
                callbackData: $"Name={options.Name}&Index={i+1}&Data={callbackData.Data}")));

        return new InlineKeyboardMarkup(buttons);
    }

    private static bool CanHaveNextButton(
        IEnumerable<string> source, 
        InlinePaginationOptions options, 
        CallbackData callbackData) 
        => source.Count() > options.MaxButtonsPerRow
            && callbackData.Index < (source.Count() - 2);

    private static bool CanHavePreviousButton(
        IEnumerable<string> source, 
        InlinePaginationOptions options, 
        CallbackData callbackData) 
        => source.Count() > options.MaxButtonsPerRow
            && callbackData.Index > 3;
}