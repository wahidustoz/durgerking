using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DurgerKing.Entity;
using Telegram.Bot;

namespace durgerking.Extentions
{
    public static class ServiceExtensions
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
