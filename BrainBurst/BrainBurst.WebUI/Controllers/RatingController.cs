using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

    public class RatingController : Controller
    {
        public IActionResult Index()
        {
            // Генеруємо 10 лідерів (заглушка до підключення БД)
            var ratings = new List<UserRatingViewModel>
            {
                new UserRatingViewModel { Position = 1, Username = "Олександр_99", Points = 1250, RankName = "Професіонал", IsCurrentUser = false },
                new UserRatingViewModel { Position = 2, Username = "Марія_К", Points = 1100, RankName = "Професіонал", IsCurrentUser = false },
                new UserRatingViewModel { Position = 3, Username = "Ivan_Student", Points = 950, RankName = "Професіонал", IsCurrentUser = false },
                new UserRatingViewModel { Position = 4, Username = "Anna_Intech", Points = 800, RankName = "Любитель", IsCurrentUser = false },
                
                // Це "Ти" - виділений користувач
                new UserRatingViewModel { Position = 5, Username = "Твій_Нікнейм", Points = 750, RankName = "Любитель", IsCurrentUser = true },

                new UserRatingViewModel { Position = 6, Username = "Petro_Z", Points = 600, RankName = "Любитель", IsCurrentUser = false },
                new UserRatingViewModel { Position = 7, Username = "Олена_С", Points = 500, RankName = "Любитель", IsCurrentUser = false },
                new UserRatingViewModel { Position = 8, Username = "Максим_123", Points = 400, RankName = "Любитель", IsCurrentUser = false },
                new UserRatingViewModel { Position = 9, Username = "Yulia_Dev", Points = 250, RankName = "Початківець", IsCurrentUser = false },
                new UserRatingViewModel { Position = 10, Username = "Дмитро_Т", Points = 100, RankName = "Початківець", IsCurrentUser = false }
            };

            return View(ratings);
        }
    }
}