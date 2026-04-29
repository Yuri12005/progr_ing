using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BrainBurst.WebUI.Controllers
{
    // Тимчасова модель для відображення набору карток
    public class DeckViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatedDate { get; set; }
        public bool IsRecent { get; set; } // Щоб знати, в яку секцію її малювати
    }

    // Додай цей клас моделі в контролер (або в окремий файл)
    public class FlashcardViewModel
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class CardsController : Controller
    {
        public IActionResult Index()
        {
            // Створюємо список карток. 
            // ПІЗНІШЕ: тут буде код на кшталт var decks = _dbContext.Decks.ToList();
            var decks = new List<DeckViewModel>
            {
                new DeckViewModel { Id = 1, Title = "дискретна математика", CreatedDate = "13/04/2023", IsRecent = false },
                new DeckViewModel { Id = 2, Title = "математичний аналіз", CreatedDate = "24/03/2023", IsRecent = false },
                new DeckViewModel { Id = 3, Title = "психологія", CreatedDate = "15/01/2025", IsRecent = false },
                new DeckViewModel { Id = 4, Title = "англійська мова", CreatedDate = "01/02/2021", IsRecent = false },
                new DeckViewModel { Id = 5, Title = "програмування", CreatedDate = "03/03/2024", IsRecent = false },
                new DeckViewModel { Id = 6, Title = "теорія ігор", CreatedDate = "01/02/2021", IsRecent = false },

                // Нещодавні
                new DeckViewModel { Id = 7, Title = "теорія ігор", CreatedDate = "01/02/2021", IsRecent = true },
                new DeckViewModel { Id = 8, Title = "психологія", CreatedDate = "15/01/2025", IsRecent = true }
            };

            // Передаємо цей список на сторінку (View)
            return View(decks);
        }

        // НОВЕ: Метод, який просто відкриває порожню сторінку створення
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // НОВЕ: Метод, який приймає дані з форми після натискання "Додати"
        [HttpPost]
        public IActionResult Create(string topic, string question, string answer)
        {
            // Тут пізніше буде логіка збереження картки в базу даних:
            // 1. Шукаємо колоду з назвою topic. Якщо немає - створюємо.
            // 2. Додаємо в неї питання (question) та відповідь (answer).

            // Після успішного "збереження" просто повертаємо користувача на список карток
            return RedirectToAction("Index");
        }

        // Метод для перегляду вмісту конкретної колоди
        public IActionResult Details(int id)
        {
            // ПІЗНІШЕ: завантаження з БД за id
            // Зараз — тестові дані для демонстрації
            var flashcards = new List<FlashcardViewModel>
            {
                new FlashcardViewModel { Question = "Що таке інкапсуляція?", Answer = "Приховування внутрішнього стану об'єкта" },
                new FlashcardViewModel { Question = "Яка різниця між класом і об'єктом?", Answer = "Клас — це креслення, об'єкт — екземпляр" },
                new FlashcardViewModel { Question = "Що таке наслідування?", Answer = "Механізм створення нового класу на основі існуючого" }
            };

            return View(flashcards);
        }
    }
}