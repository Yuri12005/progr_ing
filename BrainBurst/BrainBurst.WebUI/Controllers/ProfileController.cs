using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BrainBurst.Application.Interfaces.Services;
using BrainBurst.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace BrainBurst.WebUI.Controllers
{
    public class ArchiveTestViewModel
    {
        public int TestId { get; set; }
        public string Title { get; set; }
        public int Percent { get; set; }
        public string DateTaken { get; set; }
    }

    public class ArchiveDetailViewModel
    {
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }
        public string UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }

    [Authorize] // Закриваємо контролер для гостей
    public class ProfileController : Controller
    {
        private readonly IArchiveService _archiveService;
        private readonly UserManager<ApplicationUser> _userManager;

        // Підключаємо UserManager для роботи з профілем
        public ProfileController(IArchiveService archiveService, UserManager<ApplicationUser> userManager)
        {
            _archiveService = archiveService;
            _userManager = userManager;
        }

        // Допоміжний метод для ID
        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString!);
        }

        public async Task<IActionResult> Index()
        {
            // Беремо поточного юзера, щоб показати його стату на сторінці
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.Username = string.IsNullOrEmpty(user.FullName) ? user.UserName : user.FullName;
                ViewBag.Points = user.Points;
                ViewBag.Rank = string.IsNullOrEmpty(user.Rank) ? "Початківець" : user.Rank;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            // Підставляємо реальне ім'я в поле редагування
            ViewBag.CurrentUsername = string.IsNullOrEmpty(user?.FullName) ? user?.UserName : user?.FullName;
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Зберігаємо нове ім'я користувача
                    user.FullName = username;
                    await _userManager.UpdateAsync(user);
                }
            }
            return RedirectToAction("Index");
        }

        // РЕАЛЬНИЙ АРХІВ
        public async Task<IActionResult> Archive()
        {
            int userId = GetCurrentUserId(); // Заглушку знищено!
            var resultsDto = await _archiveService.GetArchiveAsync(userId, CancellationToken.None);

            var archive = resultsDto.Select(dto => new ArchiveTestViewModel
            {
                TestId = dto.TestResultId,
                Title = dto.Title,
                Percent = (int)Math.Round(dto.Score),
                DateTaken = dto.TestDate.ToString("dd/MM/yyyy")
            }).ToList();

            return View(archive);
        }

        // РЕАЛЬНІ ДЕТАЛІ ТЕСТУ
        // РЕАЛЬНІ ДЕТАЛІ ТЕСТУ
        public async Task<IActionResult> ArchiveDetails(int id)
        {
            // 1. Дістаємо ID поточного користувача
            int userId = GetCurrentUserId();

            // 2. Передаємо userId у сервіс разом з id тесту
            var detailsDto = await _archiveService.GetArchiveDetailsAsync(userId, id, CancellationToken.None);

            if (detailsDto == null) return NotFound();

            ViewBag.TestTitle = detailsDto.TestTitle;
            ViewBag.ScorePoints = $"{detailsDto.PointsEarned} / {detailsDto.MaxPoints}";
            ViewBag.ScorePercent = $"{Math.Round(detailsDto.ScorePercent)}%";

            var details = detailsDto.Questions.Select(q => new ArchiveDetailViewModel
            {
                QuestionText = q.QuestionText,
                CorrectAnswer = q.CorrectAnswer,
                UserAnswer = q.UserAnswer,
                IsCorrect = q.IsCorrect
            }).ToList();

            return View(details);
        }
    }
}