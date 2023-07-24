using DurgerKing.Data;
using Telegram.Bot;

namespace DurgerKing
{
    public static class ServiceCollectionExtensions
    {
        public static IApplicationBuilder UseHealth(this IApplicationBuilder app)
        {
            app.Map("/health", app =>
            {
                app.Run(async context => await context.Response.WriteAsync("BOT API/Server is working 😎"));
            });

            app.Map("/healthz", app =>
            {
                app.Run(async context =>
                {
                    var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();
                    var botClient = context.RequestServices.GetRequiredService<ITelegramBotClient>();
                    var me = await botClient.GetMeAsync();
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Database = dbContext.Database.CanConnect() ? "Healthy 😊" : "Unhealthy 😟",
                        Bot = new
                        {
                            me.Username,
                            State = me is null ? "Unhealthy 😟" : "Healthy 😊"
                        }
                    });
                });
            });

            return app;
        }
    }
}