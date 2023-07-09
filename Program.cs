using DurgerKing.Services;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// TODO: register update handler as transient
builder.Services.AddHostedService<BotStartingBackgroundService>();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
    p => new TelegramBotClient(builder.Configuration.GetValue("BotApiKey", string.Empty)));

var app = builder.Build();

app.Run();