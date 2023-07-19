using System.Text.Json.Serialization;
using DurgerKing.Entity;
using DurgerKing.Entity.Data;
using DurgerKing.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
builder.Services.AddTransient<IUpdateHandler, UpdateHandler>();
builder.Services.AddHostedService<BotStartingBackgroundService>();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
    p => new TelegramBotClient(builder.Configuration.GetValue("BotApiKey", string.Empty)));

builder.Services.AddDbContext<IAppDbContext, AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
