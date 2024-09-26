
using Microsoft.EntityFrameworkCore;
using MyGP2webapp.Data;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using MyGP2webapp.Bot.Dialogs;
using MyGP2webapp.Bot.Bots;
using MyGP2webapp.Controllers;

namespace MyGP2webapp
{
    public class Program
    {

        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add configuration sources
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add session management
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            // Add DbContext
            var connectionString = builder.Configuration.GetConnectionString("DBContextSystem");
            builder.Services.AddDbContext<DBContextSystem>(options =>
                options.UseSqlServer(connectionString));


            builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            builder.Services.AddSingleton<IStorage, MemoryStorage>();
            // Create the User state.
            builder.Services.AddSingleton<UserState>();

            builder.Services.AddSingleton<ConversationState>();
          
            builder.Services.AddScoped<MainDialog>();
            builder.Services.AddTransient<StudentController>();

            builder.Services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();
            // Continue configuring web host
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Student}/{action=Login}/{id?}");
                endpoints.MapRazorPages();
            });

            app.Run();
        }
    }
}
