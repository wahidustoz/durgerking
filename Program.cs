using DurgerKing.Entity.Data;
using DurgerKing.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<IUpdateHandler, UpdateHandler>();
builder.Services.AddHostedService<BotStartingBackgroundService>();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
    p => new TelegramBotClient(builder.Configuration.GetValue("BotApiKey", string.Empty)));

builder.Services.AddDbContext<IAppDbContext, AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

var app = builder.Build();
app.MapControllers();
app.Run();