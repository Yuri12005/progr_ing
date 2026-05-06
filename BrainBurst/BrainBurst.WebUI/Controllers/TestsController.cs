using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BrainBurst.Infrastructure.Persistence; // Підключаємо контекст бази
using BrainBurst.Domain.Entities;
using System.IO;

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

    // Класи-кур'єри для отримання даних з JavaScript
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
        public string UserInput { get; set; } // Поле для тексту
    }


    public class TestsController : Controller
    {
        // Змінна для нашої бази даних
        private readonly ApplicationDbContext _context;

        // Конструктор, через який система автоматично передає підключення до БД
        public TestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Беремо всі тести з реальної бази даних і перетворюємо їх у TestViewModel
            var tests = _context.Tests
                .Select(t => new TestViewModel
                {
                    Id = t.TestId,
                    Title = t.Title,
                    QuestionCount = 0, // Поки ставимо 0, бо зв'язку з питаннями ще немає
                    IsRecent = false
                })
                .ToList();

            return View(tests);
        }

        // 2. ПРОХОДЖЕННЯ ТЕСТУ
        public IActionResult Take(int id)
        {
            // 1. Шукаємо тест за ID, включаючи пов'язаний Тег та всі його Картки
            var test = _context.Tests
                .Include(t => t.Tag)
                    .ThenInclude(tag => tag.Flashcards)
                .FirstOrDefault(t => t.TestId == id);

            if (test == null) return NotFound();

            ViewBag.TestTitle = test.Title;

            // 2. Якщо у теста немає прив'язаної колоди (тегу)
            if (test.Tag == null)
            {
                return View(new List<TestQuestionViewModel>());
            }

            // 3. Мапимо реальні картки з бази у твою TestQuestionViewModel
            var questions = test.Tag.Flashcards.Select(f => new TestQuestionViewModel
            {
                Id = f.FlashcardId,
                QuestionText = f.Question,
                CorrectAnswer = f.Answer
            }).ToList();

            return View(questions);
        }

        // МЕТОД 1: Відкриває сторінку створення тесту
        [HttpGet]
        public IActionResult Create()
        {
            // Беремо РЕАЛЬНІ колоди (Теги) з бази даних для випадаючого списку
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

        // МЕТОД 2: Приймає дані після натискання "Створити тест"
        [HttpPost]
        public IActionResult Create(string testName, string generationType, int? selectedDeckId, IFormFile? uploadedFile)
        {
            if (string.IsNullOrEmpty(testName))
            {
                return RedirectToAction("Index");
            }

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

        // МЕТОД 3: Збереження результатів тесту (тепер він правильно всередині класу!)
        [HttpPost]
        public IActionResult SubmitResult([FromBody] TestSubmissionData data)
        {
            var currentUser = _context.Users.FirstOrDefault();
            int userId = currentUser != null ? currentUser.UserId : 1;

            var questionResultsList = new List<QuestionResult>();

            if (data.Answers != null)
            {
                foreach (var ans in data.Answers)
                {
                    questionResultsList.Add(new QuestionResult
                    {
                        FlashcardId = ans.FlashcardId,
                        IsCorrect = ans.IsCorrect,
                        UserInput = ans.UserInput ?? ""
                    });
                }
            }

            var testResult = new TestResult
            {
                TestId = data.TestId,
                UserId = userId,
                CorrectAnswersPercent = data.ScorePercent,
                QuestionResults = questionResultsList
            };

            _context.TestResults.Add(testResult);
            _context.SaveChanges();

            return Json(new { success = true });
        }
    } // <--- ОДНА закриваюча дужка для TestsController
} // <--- ОДНА закриваюча дужка для namespace