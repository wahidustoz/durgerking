using System.Globalization;
using DurgerKing.Data;
using DurgerKing.Extensions;
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
    private IBotResponseService responseService;
    private IUserService userService;
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
        using var scope = serviceScopeFactory.CreateScope();

        logger.LogInformation(
            message: "Update {updateType} received  from {userId}.",
            update.Type,
            update.Message?.From?.Id);

        dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        responseService = scope.ServiceProvider.GetRequiredService<IBotResponseService>();
        userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        var botUser = update.ExtractUser();
        var user = await userService.UpsertUserAsync(
            userId: botUser.Id,
            fullname: $"{botUser.FirstName} {botUser.LastName}",
            username: botUser.Username,
            language: botUser.LanguageCode,
            cancellationToken: cancellationToken);

        SetRequestCulture(user.Language);

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

    private static void SetRequestCulture(string language)
    {
        CultureInfo.CurrentCulture = new CultureInfo(language);
        CultureInfo.CurrentUICulture = new CultureInfo(language);
    }
}
