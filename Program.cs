using System.Text.Json.Serialization;
using DurgerKing.Dtos;
using DurgerKing.Entity;
using DurgerKing.Entity.Data;
using Durgerking.Filters;
using DurgerKing.Services;
using durgerking.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddMvcOptions(o => o.Filters.Add<AsyncFluentAutoValidation>(AsyncFluentAutoValidation.OrderLowerThanModelStateInvalidFilter))
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

builder.Services.AddLocalization();
builder.Services.AddTransient<IValidator<CreateProductDto>, CreateProductValidator>();
builder.Services.AddTransient<IValidator<UpdateProductDto>, UpdateProductValidator>();
builder.Services.AddTransient<AddressService>();
builder.Services.AddTransient<IUpdateHandler, UpdateHandler>();
builder.Services.AddHostedService<BotStartingBackgroundService>();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
    p => new TelegramBotClient(builder.Configuration.GetValue("BotApiKey", string.Empty)));

builder.Services.AddDbContext<IAppDbContext, AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddHttpClient("GeoCode", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue("GeoCodeBaseUrl", string.Empty));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
