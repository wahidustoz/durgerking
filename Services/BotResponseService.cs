using Durgerking.Services;
using DurgerKing.Resources;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DurgerKing.Services;

public class BotResponseService : IBotResponseService
{
    private readonly ILogger<BotResponseService> logger;
    private readonly IUserService userService;
    private readonly ILocalizationHandler localization;
    private readonly ITelegramBotClient botClient;

    public BotResponseService(
        ILogger<BotResponseService> logger,
        IUserService userService,
        ILocalizationHandler localization,
        ITelegramBotClient botClient)
    {
        this.logger = logger;
        this.userService = userService;
        this.localization = localization;
        this.botClient = botClient;
    }

    public async ValueTask<(long ChatId, long MessageId)> SendGreetingAsync(
        long chatId, 
        long userId, 
        CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserOrDefaultAsync(userId, cancellationToken);
        var name = user.Username ?? user.Fullname;

        logger.LogTrace("Sending a greeting to {name}", name);

        var message = await botClient.SendTextMessageAsync(
            text: localization.GetValue(Message.Greeting, name),
            chatId: chatId,
            cancellationToken: cancellationToken);

        return (chatId, message.MessageId);
    }

    public async ValueTask<(long ChatId, long MessageId)> SendLanguageSettingsAsync(
        long chatId, 
        long userId, 
        CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserOrDefaultAsync(userId, cancellationToken);
        var languagesKeyboard = new Dictionary<string, string>
        {
            { Button.LanguagesUz, $"{GetCheckmarkOrEmpty(user.Language, "uz")}O'zbekcha🇺🇿" },
            { Button.LanguagesEn, $"{GetCheckmarkOrEmpty(user.Language, "en")}English🇬🇧" },
            { Button.LanguagesRu, $"{GetCheckmarkOrEmpty(user.Language, "ru")}Русский🇷🇺" }
        }.Select(k => InlineKeyboardButton.WithCallbackData(k.Value, k.Key));
        
        var message = await botClient.SendTextMessageAsync(
            text: $"_{localization.GetValue(Button.LanguageSettings)}_",
            chatId: chatId,
            replyMarkup: new InlineKeyboardMarkup(new [] { languagesKeyboard }),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
        
        return (chatId, message.MessageId);
    }

    public async ValueTask<(long ChatId, long MessageId)> SendMainMenuAsync(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        var keyboardMatrix = new[]
        {
            new[] { Button.Settings, Button.Menu },
            new[] { Button.Orders },
        };

        var message = await botClient.SendTextMessageAsync(
            text: $"_{localization.GetValue(Message.MainMenu)}_",
            chatId: chatId,
            replyMarkup: GetInlineKeyboard(keyboardMatrix),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
        
        return (chatId, message.MessageId);
    }

    public async ValueTask<(long ChatId, long MessageId)> SendSettingsAsync(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        var keyboardMatrix = new[]
        {
            new[] { Button.LanguageSettings, Button.LocationSettings },
            new[] { Button.ContactSettings },
        };

        var message = await botClient.SendTextMessageAsync(
            text: $"_{localization.GetValue(Button.Settings)}_",
            chatId: chatId,
            replyMarkup: GetInlineKeyboard(keyboardMatrix),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
        
        return (chatId, message.MessageId);
    }

    public async ValueTask<(long ChatId, long MessageId)> SendLocationKeyboardAsync(
        long chatId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserWithLocationsOrDefaultAsync(userId, cancellationToken);

        var buttons = new List<string>();
        if(user.Locations.Any())
            buttons.Add(Button.ShowLocations);
        if(user.Locations.Count < 3)
            buttons.Add(Button.AddLocation);

        var keyboardMatrix = new[] { buttons.ToArray() };

        var message = await botClient.SendTextMessageAsync(
            text: $"_{localization.GetValue(Button.LocationSettings)}_",
            chatId: chatId,
            replyMarkup: GetInlineKeyboard(keyboardMatrix),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
        
        return (chatId, message.MessageId);
    }

    public async ValueTask<(long ChatId, long MessageId)> SendLocationRequestAsync(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        var button =  KeyboardButton.WithRequestLocation(localization.GetValue(Button.LocationRequest));
        var keyboardLayout = new[] { new[] { button } };

        var message = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: $"*{localization.GetValue(Message.LocationRequest)}*",
            replyMarkup: new ReplyKeyboardMarkup(keyboardLayout) { ResizeKeyboard = true },
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);

        return (chatId, message.MessageId);
    }

    public async ValueTask<(long ChatId, IEnumerable<long> MessageIds)> SendLocationsAsync(long chatId, long userId, CancellationToken cancellationToken = default)
    {
        var messageIds = new List<long>();
        var user = await userService.GetUserWithLocationsOrDefaultAsync(userId, cancellationToken);
        foreach(var location in user.Locations)
        {
            var deleteButton  = new[] { new[] 
            { 
                InlineKeyboardButton.WithCallbackData(
                    text: localization.GetValue(Button.DeleteAddress), 
                    callbackData: Button.DeleteAddress + $".{location.Id}") 
            } };

            var message = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: location.Address,
                replyMarkup: new InlineKeyboardMarkup(deleteButton),
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
            messageIds.Add(message.MessageId); 
        }

        return (chatId, messageIds);
    }

    private InlineKeyboardMarkup GetInlineKeyboard(string[][] matrix)
    {
        var buttonMatrix = new InlineKeyboardButton[matrix.GetLength(0)][];
        for(int i = 0; i < matrix.GetLength(0); i++)
            buttonMatrix[i] = matrix[i]
                .Select(x => InlineKeyboardButton.WithCallbackData(localization.GetValue(x), x)).ToArray();

        return new InlineKeyboardMarkup(buttonMatrix);
    }

    private static string GetCheckmarkOrEmpty(string userLanguage, string languageCode)
        => string.Equals(userLanguage, languageCode, StringComparison.InvariantCultureIgnoreCase)
        ? "✅"
        :string.Empty;
}