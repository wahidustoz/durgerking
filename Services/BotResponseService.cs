using DurgerKing.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace DurgerKing.Services;

public class BotResponseService : IBotResponseService
{
    private readonly ITelegramBotClient botClient;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private IStringLocalizer<Resources.Message> messageLocalizer;
    public BotResponseService(
        ITelegramBotClient botClient,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        this.botClient = botClient;
        this.serviceScopeFactory = serviceScopeFactory;
    }
    public async ValueTask<(long ChatId, long MessageId)> SendGreetingAsync(
        string username,
        long chatId,
        CancellationToken cancellationToken = default)
    {
        using (var scope = serviceScopeFactory.CreateScope())
        {
            messageLocalizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<Resources.Message>>();
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
            var greeting = messageLocalizer["greeting-msg", username];
            var message = await botClient.SendTextMessageAsync(
                text: greeting,
                chatId: chatId,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);

            return (message.Chat.Id, message.MessageId);
        }
    }
}