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

                // === 2. ДОДАЄМО SERILOG ДО ВЕБА ===
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllersWithViews();

                // === 3. РЕЄСТРАЦІЯ БАЗИ ДАНИХ ===
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                
                // Якщо використовуєш PostgreSQL:
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString));

                var app = builder.Build();

                // === 4. ПРЯМА ПЕРЕВІРКА ПІДКЛЮЧЕННЯ ПРИ ЗАПУСКУ ===
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    try
                    {
                        Log.Information("Checking database connection...");
                        
                        if (dbContext.Database.CanConnect())
                        {
                            Log.Information("Database connection established successfully!");
                        }
                        else
                        {
                            Log.Warning("Server found, but unable to connect to database");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error checking database connection");
                    }
                }

                // === 5. ДОДАЄМО HTTP LOGGING MIDDLEWARE ===
                // app.UseMiddleware<HttpRequestLoggingMiddleware>();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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