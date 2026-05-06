using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BrainBurst.Infrastructure.Persistence;
using BrainBurst.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BrainBurst.WebUI.Controllers
{
    // Моделі для View залишаються абсолютно такими ж, як у тебе
    public class DeckViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatedDate { get; set; }
        public bool IsRecent { get; set; } // Щоб знати, в яку секцію її малювати
    }

    public class FlashcardViewModel
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class CardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Конструктор для підключення БД
        public CardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. СПИСОК КОЛОД
        public IActionResult Index()
        {
            var decks = _context.Tags
                .Include(t => t.Flashcards) // Підтягуємо картки, щоб знати їхні дати
                .Select(t => new DeckViewModel
                {
                    Id = t.TagId,
                    Title = t.Name,
                    // Шукаємо дату останньої створеної картки. Якщо порожньо - беремо сьогоднішню.
                    CreatedDate = t.Flashcards.Any()
                        ? t.Flashcards.Max(f => f.CreatedAt).ToString("dd/MM/yyyy")
                        : DateTime.UtcNow.ToString("dd/MM/yyyy"),
                    // Нещодавні - це ті колоди, де є хоча б одна картка, створена за останні 3 дні
                    IsRecent = t.Flashcards.Any(f => f.CreatedAt > DateTime.UtcNow.AddDays(-3))
                })
                .ToList();

            return View(decks);
        }

        // 2. ВІДКРИТТЯ ФОРМИ СТВОРЕННЯ (Без змін)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. ЗБЕРЕЖЕННЯ НОВОЇ КАРТКИ В БД
        [HttpPost]
        public IActionResult Create(string topic, string question, string answer)
        {
            // Валідація: якщо щось порожнє - нічого не робимо
            if (string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
            {
                return RedirectToAction("Index");
            }

            // 1. Шукаємо колоду (тег) з назвою topic. Якщо немає - створюємо.
            var tag = _context.Tags.FirstOrDefault(t => t.Name == topic);
            if (tag == null)
            {
                tag = new Tag { Name = topic };
                _context.Tags.Add(tag);
            }

            // 2. Створюємо саму картку
            var newCard = new Flashcard
            {
                Question = question,
                Answer = answer,
                CreatedAt = DateTime.UtcNow,
                CreatorId = 1 // ТИМЧАСОВА ЗАГЛУШКА, поки не налаштована реєстрація
            };

            // 3. Зв'язуємо і зберігаємо
            newCard.Tags.Add(tag);
            _context.Flashcards.Add(newCard);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // 4. ПЕРЕГЛЯД ВМІСТУ КОНКРЕТНОЇ КОЛОДИ
        public IActionResult Details(int id)
        {
            // Шукаємо тег за його ID разом з усіма картками всередині
            var tag = _context.Tags
                .Include(t => t.Flashcards)
                .FirstOrDefault(t => t.TagId == id);

            if (tag == null) return NotFound();

            // Мапимо реальні картки у твою FlashcardViewModel
            var flashcards = tag.Flashcards.Select(f => new FlashcardViewModel
            {
                Question = f.Question,
                Answer = f.Answer
            }).ToList();

            return View(flashcards);
        }
    }
}