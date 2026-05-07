using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using BrainBurst.Infrastructure.Persistence;
using BrainBurst.Domain.Entities;
using BrainBurst.Application.Interfaces.Services; // Підключили сервіси
using System.IO;

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

    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITestService _testService; // ДОДАЛИ СЕРВІС

        // Оновлений конструктор приймає і контекст, і сервіс
        public TestsController(ApplicationDbContext context, ITestService testService)
        {
            _context = context;
            _testService = testService;
        }

        public IActionResult Index()
        {
            var tests = _context.Tests
                .Select(t => new TestViewModel
                {
                    Id = t.TestId,
                    Title = t.Title,
                    QuestionCount = 0,
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
            var availableDecks = _context.Tags
                .Include(t => t.Flashcards)
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
        public IActionResult Create(string testName, string generationType, int? selectedDeckId, IFormFile? uploadedFile)
        {
            if (string.IsNullOrEmpty(testName)) return RedirectToAction("Index");

            var currentUser = _context.Users.FirstOrDefault();
            int creatorId = currentUser != null ? currentUser.UserId : 1;
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
                    fileContent = stream.ReadToEnd();
                }

                var aiTag = new Tag { Name = testName + " (AI згенеровано)" };
                _context.Tags.Add(aiTag);
                _context.SaveChanges();

                finalTagId = aiTag.TagId;
            }
            else
            {
                return RedirectToAction("Index");
            }

            var newTest = new Test
            {
                Title = testName,
                CreatorId = creatorId,
                TagId = finalTagId
            };

            _context.Tests.Add(newTest);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ОСЬ ВІН! Метод, який тепер використовує правильний ITestService
        [HttpPost]
        public async Task<IActionResult> SubmitResult([FromBody] TestSubmissionData data)
        {
            var currentUser = _context.Users.FirstOrDefault();
            int userId = currentUser != null ? currentUser.UserId : 1;

            // Перетворюємо твої відповіді у формат (int, string), який вимагає напарник
            var answersForService = new List<(int flashcardId, string? userInput)>();
            if (data.Answers != null)
            {
                foreach (var ans in data.Answers)
                {
                    answersForService.Add((ans.FlashcardId, ans.UserInput));
                }
            }

            // Викликаємо сервіс!
            var resultDto = await _testService.SubmitAsync(
                data.TestId,
                userId,
                answersForService,
                CancellationToken.None
            );

            return Json(new { success = true, score = resultDto.CorrectAnswersPercent });
        }
    }
}