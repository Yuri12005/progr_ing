using Microsoft.AspNetCore.Mvc;
using BrainBurst.Application.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
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

    public class ProfileController : Controller
    {
        private readonly IArchiveService _archiveService;

        public ProfileController(IArchiveService archiveService)
        {
            _archiveService = archiveService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit()
        {
            ViewBag.CurrentUsername = "нікнейм";
            return View();
        }

        [HttpPost]
        public IActionResult Edit(string username)
        {
            return RedirectToAction("Index");
        }

        // РЕАЛЬНИЙ АРХІВ
        public async Task<IActionResult> Archive()
        {
            int userId = 1; // Заглушка
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
        public async Task<IActionResult> ArchiveDetails(int id)
        {
            var detailsDto = await _archiveService.GetArchiveDetailsAsync(id, CancellationToken.None);

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