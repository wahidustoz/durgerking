using System.Net;
using Durgerking.Services;
using DurgerKing.Controllers;
using DurgerKing.Extensions;
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
    private readonly IProductService productService;

    public BotResponseService(
        ILogger<BotResponseService> logger,
        IUserService userService,
        ILocalizationHandler localization,
        ITelegramBotClient botClient,
        IProductService productService)
    {
        this.logger = logger;
        this.userService = userService;
        this.localization = localization;
        this.botClient = botClient;
        this.productService = productService;
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

        var settingsButton = InlineKeyboardButton.WithCallbackData(
            text: $"🔙 {localization.GetValue(Button.Settings)}",
            callbackData: Button.Settings);

        var keyboardMatrix = new[]
        {
            languagesKeyboard,
            new[] { settingsButton },
        };
        
        var message = await botClient.SendTextMessageAsync(
            text: $"_{localization.GetValue(Button.LanguageSettings)}_",
            chatId: chatId,
            replyMarkup: new InlineKeyboardMarkup(keyboardMatrix),
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

        var keyboardMatrix = new[]
        { 
            buttons.ToArray(),
            new[] { Button.Settings }
        };

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

    public async ValueTask<(long ChatId, long MessageId)> SendLocationExceededErrorAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var message = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: localization.GetValue(Message.LocationMaxExceeded),
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);

        return (chatId, message.MessageId);
    }

    public async ValueTask<(long ChatId, long MessageId)> SendContactAsync(
        long chatId, 
        long userId, 
        CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserWithContactOrDefaultAsync(userId, cancellationToken);
        if(user.Contact is null)
            return await SendContactRequestAsync(chatId, cancellationToken);
        
        var message = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: localization.GetValue(
                key: Message.ContactDisplay, 
                arguments: new [] { user.Contact.FirstName, user.Contact.LastName, user.Contact.PhoneNumber }),
            replyMarkup: GetInlineKeyboard(new[] { new[] { Button.ContactUpdate }}),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
        
        var _ = RemoveKeyboardAsync(chatId, cancellationToken);
        
        return (chatId, message.MessageId);
    }

    public async ValueTask<(long ChatId, long MessageId)> SendContactRequestAsync(
        long chatId, 
        CancellationToken cancellationToken = default)
    {
        var button =  KeyboardButton.WithRequestContact(localization.GetValue(Button.ContactRequest));
        var keyboardLayout = new[] { new[] { button } };

        var message = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: localization.GetValue(Message.ContactRequest),
            replyMarkup: new ReplyKeyboardMarkup(keyboardLayout) { ResizeKeyboard = true },
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);

        return (chatId, message.MessageId);
    }

    private async ValueTask RemoveKeyboardAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var message = await botClient.SendTextMessageAsync(
            text: " .",
            chatId: chatId,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
        await botClient.DeleteMessageAsync(chatId, message.MessageId, cancellationToken: cancellationToken);
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

    public async ValueTask<(long ChatId, long MessageId)> SendMenuAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var keyboardMatrix = new[]
        {
            new[] { Category.Food, Category.Snack },
            new[] { Category.Drink, Category.Salad },
            new[] { Category.Set },
        };

        var message = await botClient.SendTextMessageAsync(
            text: $"_{localization.GetValue(Button.Settings)}_",
            chatId: chatId,
            replyMarkup: GetInlineKeyboard(keyboardMatrix),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);

        return (chatId, message.MessageId);
    }

    public ValueTask<(long ChatId, long MessageId)> SendCategoriesAsync(long chatId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<(long ChatId, long MessageId)> SendFoodAsync(long chatId, string queryData, CancellationToken cancellationToken = default)
    {
        var products = await productService.GetProductsAsync(1, cancellationToken);
        
        List<string> names = new();
        List<string> prices = new();
        List<byte[]> data = new();
        List<string> discount = new();
        foreach (var product in products)
        {
            names.Add(product.Name);
            prices.Add(product.Price.ToString());
            data.Add(product.Media.First().Data);
            discount.Add(product.DiscountPercentage.ToString());
        }

        var callbackData = new CallbackData(queryData);

        var message = await botClient.SendPhotoAsync(
            chatId: chatId,
            photo: Telegram.Bot.Types.InputFile.FromStream(new MemoryStream(data[callbackData.Index - 1])),
            caption: $"Name: {names[callbackData.Index - 1]}\nprice: 💲{prices[callbackData.Index - 1]}\ndiscount: ➖{discount[callbackData.Index - 1]}",
            replyMarkup: botClient.CreateInlinePagination(
                source: names,
                queryData: queryData,
                options: new InlinePaginationOptions() { Name = "pagination.foods" }  
            ),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken
        );

        return (chatId, message.MessageId);
    }

    public async ValueTask<(long ChatId, long MessageId)> SendSnackAsync(
        long chatId,
        string queryData,
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetProductsAsync(2, cancellationToken);
        
        List<string> names = new();
        List<string> prices = new();
        List<byte[]> data = new();
        List<string> discount = new();
        foreach (var product in products)
        {
            names.Add(product.Name);
            prices.Add(product.Price.ToString());
            data.Add(product.Media.First().Data);
            discount.Add(product.DiscountPercentage.ToString());
        }

        var callbackData = new CallbackData(queryData);

        var message = await botClient.SendPhotoAsync(
            chatId: chatId,
            photo: Telegram.Bot.Types.InputFile.FromStream(new MemoryStream(data[callbackData.Index - 1])),
            caption: $"Name: {names[callbackData.Index - 1]}\nprice: 💲{prices[callbackData.Index - 1]}\ndiscount: ➖{discount[callbackData.Index - 1]}",
            replyMarkup: botClient.CreateInlinePagination(
                source: names,
                queryData: queryData,
                options: new InlinePaginationOptions() { Name = "pagination.snacks" }  
            ),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken
        );

        return (chatId, message.MessageId);
    }
}