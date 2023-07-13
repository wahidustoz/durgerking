using DurgerKing.Entity.Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace DurgerKing.Services;
public partial class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> logger;
    private readonly IServiceScopeFactory serviceScopeFactory;
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
            "Update {updateType} received from {userId}.",
            update.Type,
            update.Message?.From?.Id);
        await UpsetUsersAsync(update, cancellationToken);
        var handleTask = update.Type switch
        {
            UpdateType.Message => HandleMessageAsync(botClient, update.Message, cancellationToken),
            UpdateType.EditedMessage => HandleEditedMessageAsync(botClient, update.EditedMessage, cancellationToken),
            UpdateType.CallbackQuery => HandleCallbackQueryAsync(botClient, update.CallbackQuery, cancellationToken),
            UpdateType.InlineQuery => HandleInlineQueryAsync(botClient, update.InlineQuery, cancellationToken),
            _ => HandleUnkownUpdateAsync(botClient, update, cancellationToken)
        };
        try
        {
            await handleTask;
        }
        catch(Exception ex)
        {
            await HandlePollingErrorAsync(botClient, ex, cancellationToken);
        }
    }
    private Task HandleUnkownUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    private async Task UpsetUsersAsync(Update update, CancellationToken cancellationToken)
    { 
        var telegramUser = GetUserFromUpdate(update);
        using(var scope = serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == telegramUser.Id);
            if(user is null)
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
                logger.LogInformation("New user with ID {id} update.", telegramUser.Id);
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private object GetUserIdFromUpdate(Update update)
    {
        throw new NotImplementedException();
    }

    private User GetUserFromUpdate(Update update)
    => update.Type switch
    {
        Telegram.Bot.Types.Enums.UpdateType.Message => update.Message.From,
        Telegram.Bot.Types.Enums.UpdateType.EditedMessage => update.EditedMessage.From,
        Telegram.Bot.Types.Enums.UpdateType.CallbackQuery => update.CallbackQuery.From,
        Telegram.Bot.Types.Enums.UpdateType.InlineQuery => update.InlineQuery.From,
        _=> throw new Exception("We don't support update type {update.Type}yet")
    };

}