using Microsoft.EntityFrameworkCore;
using BrainBurst.Infrastructure.Persistence;
using Serilog;
using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Application.Interfaces.Services;
using BrainBurst.Application.Services;
using BrainBurst.Infrastructure.Persistence.Repositories;
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
                // === РЕЄСТРАЦІЯ РЕПОЗИТОРІЇВ ===
                // Використовуємо єдиний стиль реєстрації через інтерфейси та їхні реалізації
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IFlashcardRepository, FlashcardRepository>();
                builder.Services.AddScoped<ITestRepository, TestRepository>();
                builder.Services.AddScoped<ITestResultRepository, TestResultRepository>();

                // ОБОВ'ЯЗКОВО ДОДАТИ: Репозиторій для колод (тегів)
                builder.Services.AddScoped<ITagRepository, TagRepository>();

                // === РЕЄСТРАЦІЯ СЕРВІСІВ ===
                builder.Services.AddScoped<IFlashcardService, FlashcardService>();
                builder.Services.AddScoped<ITestService, TestService>();

                // ОБОВ'ЯЗКОВО ДОДАТИ: Сервіс для колод (тегів)
                builder.Services.AddScoped<ITagService, TagService>();

                builder.Services.AddScoped<IArchiveService, ArchiveService>();

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
            catch (Exception ex) when (ex.GetType().Name != "HostAbortedException")
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