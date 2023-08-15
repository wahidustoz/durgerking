using System.Text.Json.Serialization;
using DurgerKing.Dtos;
using DurgerKing.Data;
using Durgerking.Filters;
using DurgerKing.Services;
using durgerking.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using DurgerKing;
using Durgerking.Services;
using DurgerKing.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddMvcOptions(options => 
        options.Filters.Add<AsyncFluentAutoValidation>(AsyncFluentAutoValidation.OrderLowerThanModelStateInvalidFilter))
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

builder.Services.AddLocalization();
builder.Services.AddTransient<IValidator<CreateProductDto>, CreateProductValidator>();
builder.Services.AddTransient<IValidator<UpdateProductDto>, UpdateProductValidator>();
builder.Services.AddTransient<IBotResponseService, BotResponseService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ILocalizationHandler, LocalizationHandler>();
builder.Services.AddTransient<IUpdateHandler, UpdateHandler>();
builder.Services.AddHostedService<BotStartingBackgroundService>();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
    p => new TelegramBotClient(builder.Configuration.GetValue("BotApiKey", string.Empty)));

builder.Services.AddDbContext<IAppDbContext, AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddDbContext<UserIdentityContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;

}).AddEntityFrameworkStores<UserIdentityContext>();


builder.Services.AddHttpClient<IAddressService, AddressService>(
    configureClient: c => c.BaseAddress = new Uri(builder.Configuration.GetValue("Geocode:BaseUrl", string.Empty)));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealth();
app.MapControllers();

app.Run();