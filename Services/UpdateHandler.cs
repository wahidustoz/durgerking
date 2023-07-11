using DurgerKing.Entity.Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace DurgerKing.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> logger;
    private readonly IServiceScopeFactory serviceScopeFactory;
    public UpdateHandler(
        ILogger<UpdateHandler> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.serviceScopeFactory=serviceScopeFactory;
    }
    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Polling error happened.");
        return Task.CompletedTask;
    }
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Update {updateType} received from {userId}.",
            update.Type,
            update.Message?.From?.Id);
        //return Task.CompletedTask;
        try
        {
            await UpsetUserAsync(update, cancellationToken);
        }
        catch (Exception ex)
        {
            HandlePollingErrorAsync(botClient, ex, cancellationToken);
        }
    }
    private async Task UpsetUserAsync(Update update, CancellationToken cancellationToken)
    {
        var telegramUser = GetUserFromUpdate(update);
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == telegramUser.Id);
            if (user is null)
            {
                dbContext.Users.Add(new DurgerKing.Entity.User
                {
                    Id = telegramUser.Id,
                    Fullname = $"{telegramUser.FirstName} {telegramUser.LastName}",
                    Username = telegramUser.Username,
                    Language = telegramUser.LanguageCode,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                });
                logger.LogInformation("New user with ID {id} added.", telegramUser.Id);
            }
            else
            {
                user.Fullname = $"{telegramUser.FirstName} {telegramUser.LastName}";
                user.Username = telegramUser.Username;
                user.Language = telegramUser.LanguageCode;
                user.CreatedAt = DateTime.UtcNow;
                user.ModifiedAt = DateTime.UtcNow;
                logger.LogInformation("New user with ID {id} updated.", telegramUser.Id);
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    private User GetUserFromUpdate(Update update)
        => update.Type switch
        {
            Telegram.Bot.Types.Enums.UpdateType.Message => update.Message.From,
            Telegram.Bot.Types.Enums.UpdateType.EditedMessage => update.EditedMessage.From,
            Telegram.Bot.Types.Enums.UpdateType.CallbackQuery => update.CallbackQuery.From,
            Telegram.Bot.Types.Enums.UpdateType.InlineQuery => update.InlineQuery.From,
            _ => throw new Exception("We don't support update.Type {update.Type} yet.")
        };
}