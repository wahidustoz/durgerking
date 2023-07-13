using Telegram.Bot;
using Telegram.Bot.Polling;
namespace DurgerKing.Services;
public class BotStartingBackgroundService : BackgroundService
{
    private readonly ILogger<BotStartingBackgroundService> logger;
    private readonly ITelegramBotClient botClient;
    private readonly IUpdateHandler updateHandler;
    public BotStartingBackgroundService(
        ILogger<BotStartingBackgroundService> logger,
        ITelegramBotClient botClient,
        IUpdateHandler updateHandler)
    {
        this.logger = logger;
        this.botClient = botClient;
        this.updateHandler = updateHandler;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var bot = await botClient.GetMeAsync(cancellationToken);
            logger.LogInformation("Bot {username} started polling updates.", bot.Username);

            botClient.StartReceiving(
                updateHandler : updateHandler,
                receiverOptions : default,
                cancellationToken : cancellationToken
            );
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to connect to bot server.");
        }
    }
}