using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BrainBurst.Application.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Security.Claims; // Обов'язково для роботи з Claims (кукісами)

namespace BrainBurst.WebUI.Controllers
{
    public class DeckViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatedDate { get; set; }
        public bool IsRecent { get; set; }
    }

    public class FlashcardViewModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    [Authorize] // Захищаємо контролер від неавторизованих користувачів
    public class CardsController : Controller
    {
        private readonly IFlashcardService _flashcardService;
        private readonly ITagService _tagService;

        public CardsController(IFlashcardService flashcardService, ITagService tagService)
        {
            _flashcardService = flashcardService;
            _tagService = tagService;
        }

        // Допоміжний метод для зручного отримання ID поточного користувача
        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString!);
        }

        public async Task<IActionResult> Index()
        {
            var tags = await _tagService.GetDecksAsync(CancellationToken.None);

            var decks = tags.Select(t => new DeckViewModel
            {
                Id = t.TagId,
                Title = t.Name,
                CreatedDate = t.LastCardCreatedAt?.ToString("dd/MM/yyyy") ?? DateTime.UtcNow.ToString("dd/MM/yyyy"),
                IsRecent = t.LastCardCreatedAt.HasValue && t.LastCardCreatedAt.Value > DateTime.UtcNow.AddDays(-3)
            }).ToList();

            return View(decks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string topic, string question, string answer)
        {
            if (string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
            {
                return RedirectToAction("Index");
            }

            // БЕРЕМО РЕАЛЬНИЙ ID ЗАМІСТЬ ЗАГЛУШКИ
            int creatorId = GetCurrentUserId(); 
            var tags = new List<string> { topic };

            await _flashcardService.CreateAsync(creatorId, question, answer, tags, CancellationToken.None);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var tag = await _tagService.GetDeckDetailsAsync(id, CancellationToken.None);

            if (tag == null) return NotFound();

            var flashcards = tag.Flashcards.Select(f => new FlashcardViewModel
            {
                Id = f.FlashcardId,
                Question = f.Question,
                Answer = f.Answer
            }).ToList();

            return View(flashcards);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCard(int cardId, int deckId)
        {
            // БЕРЕМО РЕАЛЬНИЙ ID ЗАМІСТЬ ЗАГЛУШКИ
            int requesterId = GetCurrentUserId(); 

            // Викликаємо метод видалення, який ми протестували
            await _flashcardService.DeleteAsync(cardId, requesterId, CancellationToken.None);

            // Перезавантажуємо сторінку цієї ж колоди
            return RedirectToAction("Details", new { id = deckId });
        }
    }
}