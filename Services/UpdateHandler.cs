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
        using (var scope = serviceScopeFactory.CreateScope())
        {
            dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();    
            logger.LogInformation(
            "Update {updateType} received  from {userId}.",
            update.Type,
            update.Message?.From?.Id);

        await UpsertUserAsync(update, cancellationToken);

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
        catch(Exception ex)
        {
            await HandlePollingErrorAsync(botClient, ex, cancellationToken);
        }
    }}

    private async Task UpsertUserAsync(Update update, CancellationToken cancellationToken)
    {
        var telegramUser = GetUserFromUpdate(update);

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
            user.ModifiedAt = DateTime.UtcNow;
            logger.LogInformation("user with ID {id} updated.", telegramUser.Id);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private User GetUserFromUpdate(Update update)
        => update.Type switch
        {
            UpdateType.Message => update.Message.From,
            UpdateType.EditedMessage => update.EditedMessage.From,
            UpdateType.CallbackQuery => update.CallbackQuery.From,
            UpdateType.InlineQuery => update.InlineQuery.From,
            _ => throw new Exception("We dont supportas update type {update.Type} yet") 
        };   
}
