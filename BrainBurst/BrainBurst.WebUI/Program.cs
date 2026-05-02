using Microsoft.EntityFrameworkCore;
using BrainBurst.Infrastructure.Persistence;
using Serilog;

namespace BrainBurst.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // === 1. НАЛАШТУВАННЯ SERILOG ===
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            try
            {
                Log.Information("Application BrainBurst starting...");

                var builder = WebApplication.CreateBuilder(args);

                // Підключаємо Serilog до хоста
                builder.Host.UseSerilog();

                // Додаємо сервіси MVC
                builder.Services.AddControllersWithViews();

                // === 2. РЕЄСТРАЦІЯ БАЗИ ДАНИХ ===
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString));
                // === РЕЄСТРАЦІЯ РЕПОЗИТОРІЇВ ===
                builder.Services.AddScoped<BrainBurst.Application.Interfaces.Repositories.IUserRepository, BrainBurst.Infrastructure.Persistence.Repositories.UserRepository>();
                builder.Services.AddScoped<BrainBurst.Application.Interfaces.Repositories.IFlashcardRepository, BrainBurst.Infrastructure.Persistence.Repositories.FlashcardRepository>();
                builder.Services.AddScoped<BrainBurst.Application.Interfaces.Repositories.ITestRepository, BrainBurst.Infrastructure.Persistence.Repositories.TestRepository>();
                builder.Services.AddScoped<BrainBurst.Application.Interfaces.Repositories.ITestResultRepository, BrainBurst.Infrastructure.Persistence.Repositories.TestResultRepository>();

                var app = builder.Build();

                // === 3. НАЛАШТУВАННЯ HTTP PIPELINE ===
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseRouting();

                app.UseAuthorization();

                app.MapStaticAssets();
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                    .WithStaticAssets();

                Log.Information("Application started successfully!");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Critical error starting application");
            }
            finally
            {
                Log.Information("Application shutting down...");
                Log.CloseAndFlush();
            }
        }
    }
}