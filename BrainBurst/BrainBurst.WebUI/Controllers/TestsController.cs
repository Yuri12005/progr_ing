using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BrainBurst.WebUI.Controllers
{
    // Тимчасова модель для відображення тесту
    public class TestViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int QuestionCount { get; set; }
        public bool IsRecent { get; set; }
    }

    public class TestQuestionViewModel
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }
    }


    public class TestsController : Controller
    {
        public IActionResult Index()
        {
            // Створюємо список тестів (заглушка до підключення БД)
            var tests = new List<TestViewModel>
            {
                new TestViewModel { Id = 1, Title = "дискретна математика", QuestionCount = 27, IsRecent = false },
                new TestViewModel { Id = 2, Title = "математичний аналіз", QuestionCount = 106, IsRecent = false },
                new TestViewModel { Id = 3, Title = "психологія", QuestionCount = 244, IsRecent = false },
                new TestViewModel { Id = 4, Title = "англійська мова", QuestionCount = 190, IsRecent = false },
                new TestViewModel { Id = 5, Title = "програмування", QuestionCount = 32, IsRecent = false },
                new TestViewModel { Id = 6, Title = "теорія ігор", QuestionCount = 56, IsRecent = false },

                // Нещодавні
                new TestViewModel { Id = 7, Title = "теорія ігор", QuestionCount = 56, IsRecent = true },
                new TestViewModel { Id = 8, Title = "психологія", QuestionCount = 244, IsRecent = true }
            };

            return View(tests);
        }

        public IActionResult Take(int id)
        {
            // Заглушка: список питань для перевірки UI
            var questions = new List<TestQuestionViewModel>
            {
                new TestQuestionViewModel { Id = 1, QuestionText = "Що таке поліморфізм?", CorrectAnswer = "Здатність об'єктів різних класів реагувати на однакові виклики методів" },
                new TestQuestionViewModel { Id = 2, QuestionText = "Скільки байтів займає тип int у C#?", CorrectAnswer = "4" },
                new TestQuestionViewModel { Id = 3, QuestionText = "Який модифікатор доступу робить член класу доступним лише всередині цього ж класу?", CorrectAnswer = "private" }
            };

            // Передаємо назву тесту через ViewBag для заголовка
            ViewBag.TestTitle = "Програмування C#";

            return View(questions);
        }

        // МЕТОД 1: Відкриває сторінку створення тесту
        [HttpGet]
        public IActionResult Create()
        {
            // Беремо кілька колод для прикладу, щоб показати їх на сторінці вибору
            var availableDecks = new List<TestViewModel>
            {
                new TestViewModel { Id = 1, Title = "дискретна математика", QuestionCount = 27 },
                new TestViewModel { Id = 2, Title = "математичний аналіз", QuestionCount = 106 },
                new TestViewModel { Id = 3, Title = "психологія", QuestionCount = 244 }
            };

            return View(availableDecks);
        }

        // МЕТОД 2: Приймає дані після натискання "Створити тест"
        [HttpPost]
        public IActionResult Create(string testName, string generationType, int? selectedDeckId)
        {
            // ПІЗНІШЕ ТУТ БУДЕ ЛОГІКА:
            // Якщо generationType == "file" -> відправляємо файл в OpenAI
            // Якщо generationType == "deck" -> беремо питання з колоди selectedDeckId

            return RedirectToAction("Index");
        }
    }
}