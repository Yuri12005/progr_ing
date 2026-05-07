using BrainBurst.Application.DTOs;
using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Application.Interfaces.Services;

namespace BrainBurst.Application.Services;

public class ArchiveService : IArchiveService
{
    private readonly ITestResultRepository _testResultRepository;

    public ArchiveService(ITestResultRepository testResultRepository)
    {
        _testResultRepository = testResultRepository;
    }

    public async Task<IReadOnlyList<ArchiveEntryDTO>> GetArchiveAsync(int userId, CancellationToken ct)
    {
        var results = await _testResultRepository.GetByUserAsync(userId, ct);

        return results.Select(r => new ArchiveEntryDTO
        {
            TestResultId = r.TestResultId,
            Title = r.Test?.Title ?? "Тест без назви",
            TestDate = r.TestDate,
            Score = r.CorrectAnswersPercent,
            Points = r.Points
        }).ToList();
    }

    public async Task<ArchiveDetailsDTO?> GetArchiveDetailsAsync(int testResultId, CancellationToken ct)
    {
        // Оскільки в репозиторії немає GetById, ми використовуємо GetByUserAsync (тимчасове рішення, ID=1)
        var allResults = await _testResultRepository.GetByUserAsync(1, ct);
        var result = allResults.FirstOrDefault(r => r.TestResultId == testResultId);

        if (result == null) return null;

        int earned = result.QuestionResults.Count(q => q.IsCorrect);
        int max = result.QuestionResults.Count;

        return new ArchiveDetailsDTO
        {
            TestTitle = result.Test?.Title ?? "Тест без назви",
            PointsEarned = earned,
            MaxPoints = max,
            ScorePercent = result.CorrectAnswersPercent,
            Questions = result.QuestionResults.Select(q => new ArchiveQuestionDTO
            {
                QuestionText = q.Flashcard?.Question ?? "Питання було видалено",
                CorrectAnswer = q.Flashcard?.Answer ?? "Невідомо",
                UserAnswer = q.UserInput ?? "",
                IsCorrect = q.IsCorrect
            }).ToList()
        };
    }
}