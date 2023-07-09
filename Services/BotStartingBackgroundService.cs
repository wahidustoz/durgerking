using Telegram.Bot;

namespace DurgerKing.Services;

public class BotStartingBackgroundService : BackgroundService
{
    private readonly ILogger<BotStartingBackgroundService> logger;
    private readonly ITelegramBotClient botClient;

    public BotStartingBackgroundService(
        ILogger<BotStartingBackgroundService> logger,
        ITelegramBotClient botClient)
    {
        this.logger = logger;
        this.botClient = botClient;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var bot = await botClient.GetMeAsync(cancellationToken);
            logger.LogInformation("Bot {username} started polling updates.", bot.Username);

            // TODO: Register update handler and start receiving updates
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to connect to bot server.");
        }
    }
}