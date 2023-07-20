using System.Globalization;
using DurgerKing.Entity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DurgerKing.Services;

public partial class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> logger;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private IStringLocalizer<Resources.Message> messageLocalizer;
    private IStringLocalizer<Resources.Control> controlLocalizer;
    private IAppDbContext dbContext;

    public UpdateHandler(
        ILogger<UpdateHandler> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Polling error happened.");
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        logger.LogInformation(
        "Update {updateType} received  from {userId}.",
        update.Type,
        update.Message?.From?.Id);

        using (var scope = serviceScopeFactory.CreateScope())
        {
            dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            messageLocalizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<Resources.Message>>();
            controlLocalizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<Resources. Control>>();

            var user = await UpsertUserAsync(update, cancellationToken);
            CultureInfo.CurrentCulture = new CultureInfo(user.Language);
            CultureInfo.CurrentUICulture = new CultureInfo(user.Language);

            var handleTask = update.Type switch
            {
                UpdateType.Message => HandleMessageAsync(botClient, update.Message, cancellationToken),
                UpdateType.CallbackQuery => HandleCallBackQueryAsync(botClient, update.CallbackQuery, cancellationToken),
                _ => throw new NotImplementedException()
            };

            try
            {
                await handleTask;
            }
            catch (Exception ex)
            {
                await HandlePollingErrorAsync(botClient, ex, cancellationToken);
            }
        }
    }

    private async Task<Entity.User> UpsertUserAsync(Update update, CancellationToken cancellationToken)
    {
        var telegramUser = GetUserFromUpdate(update);
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == telegramUser.Id, cancellationToken);
        if (user is null)
        {
            user = new DurgerKing.Entity.User
            {
                Id = telegramUser.Id,
                Fullname = $"{telegramUser.FirstName} {telegramUser.LastName}",
                Username = telegramUser.Username,
                Language = telegramUser.LanguageCode,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            dbContext.Users.Add(user);
            logger.LogInformation("New user with ID {id} added.", telegramUser.Id);
        }
        else
        {
            user.Fullname = $"{telegramUser.FirstName} {telegramUser.LastName}";
            user.Username = telegramUser.Username;
            user.ModifiedAt = DateTime.UtcNow;
            logger.LogInformation("user with ID {id} updated.", telegramUser.Id);
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    private static User GetUserFromUpdate(Update update)
        => update.Type switch
        {
            UpdateType.Message => update.Message.From,
            UpdateType.EditedMessage => update.EditedMessage.From,
            UpdateType.CallbackQuery => update.CallbackQuery.From,
            UpdateType.InlineQuery => update.InlineQuery.From,
            _ => throw new Exception("We dont supportas update type {update.Type} yet")
        };
}
