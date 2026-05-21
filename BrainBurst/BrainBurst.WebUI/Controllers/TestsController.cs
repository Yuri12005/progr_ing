using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using BrainBurst.Infrastructure.Persistence;
using BrainBurst.Domain.Entities;
using BrainBurst.Application.Interfaces.Services;
using System.IO;
using System.Security.Claims; // Для роботи з кукісами та ID

namespace BrainBurst.WebUI.Controllers
{
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

    public class TestSubmissionData
    {
        public int TestId { get; set; }
        public decimal ScorePercent { get; set; }
        public List<AnswerDetail> Answers { get; set; }
    }

    public class AnswerDetail
    {
        public int FlashcardId { get; set; }
        public bool IsCorrect { get; set; }
        public string UserInput { get; set; }
    }

    [Authorize] // Закриваємо контролер від гостей
    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITestService _testService;
        private readonly ITestGenerationService _testGenerationService;

        public TestsController(ApplicationDbContext context, ITestService testService, ITestGenerationService testGenerationService)
        {
            _context = context;
            _testService = testService;
            _testGenerationService = testGenerationService; // Ініціалізація
        }

        // Допоміжний метод для отримання реального ID
        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString!);
        }

        public IActionResult Index(string searchQuery)
        {
            int currentUserId = GetCurrentUserId();

            // 1. Створюємо базовий запит (беремо тільки тести цього користувача)
            var query = _context.Tests.Where(t => t.CreatorId == currentUserId);

            // 2. ЛОГІКА ПОШУКУ: якщо є текст, фільтруємо базу даних
            if (!string.IsNullOrEmpty(searchQuery))
            {
                // Використовуємо ToLower(), щоб пошук не залежав від великих/малих літер
                query = query.Where(t => t.Title.ToLower().Contains(searchQuery.ToLower()));

                // Зберігаємо запит для відображення в інпуті
                ViewData["SearchQuery"] = searchQuery;
            }

            // 3. Виконуємо запит і формуємо список для View
            var tests = query
                .Select(t => new TestViewModel
                {
                    Id = t.TestId,
                    Title = t.Title,
                    QuestionCount = t.Tag != null ? t.Tag.Flashcards.Count : 0,
                    IsRecent = false
                })
                .ToList();

            return View(tests);
        }

        public IActionResult Take(int id)
        {
            var test = _context.Tests
                .Include(t => t.Tag)
                    .ThenInclude(tag => tag.Flashcards)
                .FirstOrDefault(t => t.TestId == id);

            if (test == null) return NotFound();

            ViewBag.TestTitle = test.Title;

            if (test.Tag == null)
            {
                return View(new List<TestQuestionViewModel>());
            }

            var questions = test.Tag.Flashcards.Select(f => new TestQuestionViewModel
            {
                Id = f.FlashcardId,
                QuestionText = f.Question,
                CorrectAnswer = f.Answer
            }).ToList();

            return View(questions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            int currentUserId = GetCurrentUserId();

            var availableDecks = _context.Tags
                .Include(t => t.Flashcards)
                .Where(t => t.CreatorId == currentUserId) // Тільки колоди поточного юзера
                .Select(t => new TestViewModel
                {
                    Id = t.TagId,
                    Title = t.Name,
                    QuestionCount = t.Flashcards.Count
                })
                .ToList();

            return View(availableDecks);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string testName, string generationType, int? selectedDeckId, IFormFile? uploadedFile)
        {
            if (string.IsNullOrEmpty(testName)) return RedirectToAction("Index");

            int creatorId = GetCurrentUserId();
            int? finalTagId = null;

            if (generationType == "deck" && selectedDeckId.HasValue)
            {
                finalTagId = selectedDeckId.Value;
            }
            else if (generationType == "file" && uploadedFile != null && uploadedFile.Length > 0)
            {
                string fileContent;
                using (var stream = new StreamReader(uploadedFile.OpenReadStream()))
                {
                    fileContent = await stream.ReadToEndAsync();
                }

                try
                {
                    // 1. СПОЧАТКУ звертаємося до ШІ (ще нічого не зберігаємо в базу!)
                    var generatedCardsDto = await _testGenerationService.CreateFlashcardsFromTextAsync(
                        creatorId,
                        fileContent,
                        new List<string>(),
                        CancellationToken.None);

                    // 2. Якщо ШІ повернув порожній результат - перериваємо створення
                    if (generatedCardsDto == null || !generatedCardsDto.Any())
                    {
                        // Можеш вивести це повідомлення через TempData у своїй View, якщо хочеш
                        TempData["Error"] = "ШІ не зміг згенерувати картки з цього тексту.";
                        return RedirectToAction("Create");
                    }

                    // 3. ТІЛЬКИ ТЕПЕР, коли маємо картки, створюємо колоду
                    var aiTag = new Tag
                    {
                        Name = testName + " (AI згенеровано)",
                        CreatorId = creatorId
                    };
                    _context.Tags.Add(aiTag);

                    // 4. Додаємо картки
                    foreach (var dto in generatedCardsDto)
                    {
                        var newCard = new Flashcard
                        {
                            Question = dto.Question,
                            Answer = dto.Answer,
                            CreatorId = creatorId
                        };
                        newCard.Tags.Add(aiTag);
                        _context.Flashcards.Add(newCard);
                    }

                    // Зберігаємо колоду і картки одним махом
                    await _context.SaveChangesAsync();
                    finalTagId = aiTag.TagId;
                }
                catch (Exception ex)
                {
                    // Якщо ШІ впав (ASCII помилка, немає грошей і т.д.)
                    Serilog.Log.Error(ex, "КРИТИЧНА ПОМИЛКА ШІ ПРИ ГЕНЕРАЦІЇ");
                    // Повертаємо назад на форму, НІЧОГО не зберігши в базу!
                    return RedirectToAction("Create");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

            // ЦЕЙ КОД ВИКОНАЄТЬСЯ ТІЛЬКИ ЯКЩО ВСЕ ПРОЙШЛО УСПІШНО АБО ЯКЩО ОБРАНО ГОТОВУ КОЛОДУ
            var newTest = new Test
            {
                Title = testName,
                CreatorId = creatorId,
                TagId = finalTagId
            };

            _context.Tests.Add(newTest);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

[HttpPost]
public async Task<IActionResult> SubmitResult([FromBody] TestSubmissionData data)
{
    int userId = GetCurrentUserId(); // Беремо реальний ID

    var answersForService = new List<(int flashcardId, string? userInput)>();
    int correctAnswersCount = 0; // Лічильник правильних відповідей

    if (data.Answers != null)
    {
        foreach (var ans in data.Answers)
        {
            answersForService.Add((ans.FlashcardId, ans.UserInput));
            if (ans.IsCorrect)
            {
                correctAnswersCount++;
            }
        }
    }

    // 1. Зберігаємо результат тесту через сервіс
    var resultDto = await _testService.SubmitAsync(
        data.TestId,
        userId,
        answersForService,
        CancellationToken.None
    );

    // 2. НАРАХОВУЄМО БАЛИ КОРИСТУВАЧУ
    var user = await _context.Users.FindAsync(userId);
    if (user != null)
    {
        int earnedPoints = correctAnswersCount * 10; // 10 балів за кожну правильну відповідь
        user.Points += earnedPoints;

        // 3. АВТОМАТИЧНО ОНОВЛЮЄМО РАНГ
        if (user.Points >= 1000)
        {
            user.Rank = "Професіонал";
        }
        else if (user.Points >= 300)
        {
            user.Rank = "Досвідчений";
        }
        else
        {
            user.Rank = "Початківець";
        }

        // Зберігаємо зміни юзера в базу даних
        await _context.SaveChangesAsync();
    }

    return Json(new { success = true, score = resultDto.CorrectAnswersPercent, earned = correctAnswersCount * 10 });
}
    }
}