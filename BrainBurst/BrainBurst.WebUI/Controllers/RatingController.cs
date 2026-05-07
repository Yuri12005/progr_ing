using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BrainBurst.Infrastructure.Persistence;

namespace BrainBurst.WebUI.Controllers
{
    // Модель для одного рядка в таблиці рейтингу
    public class UserRatingViewModel
    {
        public int Position { get; set; }
        public string Username { get; set; }
        public int Points { get; set; }
        public string RankName { get; set; }
        public bool IsCurrentUser { get; set; } // Щоб виділити рядок кольором
    }

    [Authorize] // Тільки для залогінених користувачів
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Підключаємо базу даних
        public RatingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Отримуємо ID поточного користувача
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int currentUserId = int.Parse(userIdString!);

            // Витягуємо всіх користувачів з бази, сортуємо за балами (за спаданням)
            // Примітка: переконайся, що у твоєму класі ApplicationUser є властивість Points!
            var allUsers = await _context.Users
                .OrderByDescending(u => u.Points) 
                .ToListAsync();

            var ratings = new List<UserRatingViewModel>();
            int currentPosition = 1;

            foreach (var user in allUsers)
            {
                ratings.Add(new UserRatingViewModel
                {
                    Position = currentPosition,
                    // Якщо в юзера заповнене FullName — показуємо його, якщо ні — беремо логін (UserName)
                    Username = string.IsNullOrEmpty(user.FullName) ? user.UserName : user.FullName,
                    Points = user.Points, 
                    // Якщо ранг ще не призначений, ставимо стандартний
                    RankName = string.IsNullOrEmpty(user.Rank) ? "Початківець" : user.Rank,
                    IsCurrentUser = user.Id == currentUserId
                });
                
                currentPosition++;
            }

            // Якщо хочеш показувати лише Топ-50 гравців, розкоментуй цей рядок:
            // ratings = ratings.Take(50).ToList();

            return View(ratings);
        }
    }
}