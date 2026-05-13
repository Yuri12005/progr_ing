using Microsoft.EntityFrameworkCore;
using BrainBurst.Infrastructure.Persistence;
using Serilog;
using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Application.Interfaces.Services;
using BrainBurst.Application.Services;
using BrainBurst.Infrastructure.Persistence.Repositories;
using BrainBurst.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BrainBurst.WebUI
{
    public class Program
    {
        public static async Task Main(string[] args)
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

                // === 2. РЕЄСТРАЦІЯ БАЗИ ДАНИХ ТА IDENTITY ===
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString));

                // Підключення Identity
                builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
                {
                    // Спрощені налаштування пароля для зручності тестування
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;

                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

                // Налаштування кукі (куди перекидати, якщо не залогінений)
                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Залишатися в системі 7 днів
                });

                // === РЕЄСТРАЦІЯ РЕПОЗИТОРІЇВ ===
                // Використовуємо єдиний стиль реєстрації через інтерфейси та їхні реалізації
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IFlashcardRepository, FlashcardRepository>();
                builder.Services.AddScoped<ITestRepository, TestRepository>();
                builder.Services.AddScoped<ITestResultRepository, TestResultRepository>();
                builder.Services.AddScoped<ITagRepository, TagRepository>();

                // === РЕЄСТРАЦІЯ СЕРВІСІВ ===
                builder.Services.AddScoped<IFlashcardService, FlashcardService>();
                builder.Services.AddScoped<ITestService, TestService>();
                builder.Services.AddScoped<ITagService, TagService>();
                builder.Services.AddScoped<IArchiveService, ArchiveService>();


                var app = builder.Build();

                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        // Викликаємо наш новий Seeder
                        await BrainBurst.WebUI.Data.DbSeeder.SeedRolesAndAdminAsync(services);
                        Log.Information("Data seeding completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "An error occurred while seeding the database.");
                    }
                }

                // === 3. НАЛАШТУВАННЯ HTTP PIPELINE ===
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseRouting();

                // ОБОВ'ЯЗКОВО: Authentication має бути перед Authorization
                app.UseAuthentication(); 
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