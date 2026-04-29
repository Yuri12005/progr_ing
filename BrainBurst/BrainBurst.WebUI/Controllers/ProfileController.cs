using Microsoft.AspNetCore.Mvc;

namespace BrainBurst.WebUI.Controllers
{
    // Модель для списку пройдених тестів (Архів)
    public class ArchiveTestViewModel
    {
        public int TestId { get; set; }
        public string Title { get; set; }
        public int Percent { get; set; }
        public string DateTaken { get; set; }
    }

    // Модель для детального розбору одного пройденого тесту
    public class ArchiveDetailViewModel
    {
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }
        public string UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class ProfileController : Controller
    {
        // Відкриває головну сторінку профілю
        public IActionResult Index()
        {
            return View();
        }

        // Відкриває сторінку редагування
        [HttpGet]
        public IActionResult Edit()
        {
            // Передаємо поточне ім'я користувача для відображення в полі
            ViewBag.CurrentUsername = "нікнейм";
            return View();
        }

        // Зберігає зміни і повертає назад у профіль
        [HttpPost]
        public IActionResult Edit(string username)
        {
            // ПІЗНІШЕ: тут буде код оновлення імені (та фото) в базі даних
            return RedirectToAction("Index");
        }

        // НОВЕ: Метод, що відкриває список архіву
        public IActionResult Archive()
        {
            // Заглушка: список пройдених тестів
            var archive = new List<ArchiveTestViewModel>
            {
                new ArchiveTestViewModel { TestId = 1042, Title = "Основи C#", Percent = 100, DateTaken = "28/04/2026" },
                new ArchiveTestViewModel { TestId = 953, Title = "Дискретна математика", Percent = 66, DateTaken = "25/04/2026" },
                new ArchiveTestViewModel { TestId = 712, Title = "Психологія", Percent = 85, DateTaken = "15/03/2026" }
            };

            return View(archive);
        }

        // НОВЕ: Метод, що відкриває результати конкретного тесту з архіву
        public IActionResult ArchiveDetails(int id)
        {
            // Заглушка: деталі конкретного тесту
            ViewBag.TestTitle = "Дискретна математика (Архів)";
            ViewBag.ScorePoints = "2 / 3";
            ViewBag.ScorePercent = "66%";

            var details = new List<ArchiveDetailViewModel>
            {
                new ArchiveDetailViewModel { QuestionText = "Що таке граф?", CorrectAnswer = "Множина вершин і ребер", UserAnswer = "Множина вершин і ребер", IsCorrect = true },
                new ArchiveDetailViewModel { QuestionText = "Що таке дерево в теорії графів?", CorrectAnswer = "Зв'язний граф без циклів", UserAnswer = "Просто граф", IsCorrect = false },
                new ArchiveDetailViewModel { QuestionText = "Скільки ребер у повному графі з 4 вершин?", CorrectAnswer = "6", UserAnswer = "6", IsCorrect = true }
            };

            return View(details);
        }

    }
}