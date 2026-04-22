using Microsoft.EntityFrameworkCore;
using BrainBurst.Infrastructure.Persistence; // Підключаємо твій DbContext

namespace BrainBurst.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // === 1. РЕЄСТРАЦІЯ БАЗИ ДАНИХ (БЕЗ ЦЬОГО НЕ ПРАЦЮВАТИМЕ) ===
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            // Якщо використовуєш PostgreSQL:
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
                
            // (Якщо MS SQL, зміни UseNpgsql на UseSqlServer)
            // ============================================================

            var app = builder.Build();

            // === 2. ПРЯМА ПЕРЕВІРКА ПІДКЛЮЧЕННЯ ПРИ ЗАПУСКУ ===
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                try
                {
                    Console.WriteLine("\n⏳ Перевіряю підключення до бази даних...");
                    
                    if (dbContext.Database.CanConnect())
                    {
                        Console.WriteLine("✅ СУПЕР! Зв'язок з базою даних ВСТАНОВЛЕНО!\n");
                    }
                    else
                    {
                        Console.WriteLine("❌ Сервер знайдено, але до бази підключитися не вдалося (можливо, ти її ще не створив вручну).\n");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ ПОМИЛКА: Немає зв'язку з сервером. Перевір пароль або чи запущений сам сервер БД.");
                    Console.WriteLine($"Деталі: {ex.Message}\n");
                }
            }
            // ===================================================

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

            app.Run();
        }
    }
}