using BrainBurst.Application.DTOs;
using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Application.Interfaces.Services;
using BrainBurst.Domain.Entities;

namespace BrainBurst.Application.Services;

public class TestService : ITestService
{
    // Замість DbContext використовуємо репозиторії!
    private readonly ITestRepository _testRepository;
    private readonly ITestResultRepository _testResultRepository;
    private readonly IFlashcardRepository _flashcardRepository;

    public TestService(
        ITestRepository testRepository,
        ITestResultRepository testResultRepository,
        IFlashcardRepository flashcardRepository)
    {
        _testRepository = testRepository;
        _testResultRepository = testResultRepository;
        _flashcardRepository = flashcardRepository;
    }

    public async Task<TestDTO?> GetAsync(int id, CancellationToken ct)
    {
        var test = await _testRepository.GetAsync(id, ct);
        if (test == null) return null;

        // Мапимо сутність у DTO
        var flashcardsDto = test.Tag?.Flashcards?.Select(f => new FlashcardDTO
        {
            FlashcardId = f.FlashcardId,
            Question = f.Question,
            Answer = f.Answer,
            Tags = new List<string> { test.Tag.Name }
        }).ToList() ?? new List<FlashcardDTO>();

        return new TestDTO
        {
            TestId = test.TestId,
            CreatorId = test.CreatorId,
            Flashcards = flashcardsDto
        };
    }

    public async Task<TestDTO> GenerateFromFlashcardsAsync(int creatorId, IEnumerable<int> flashcardIds, CancellationToken ct)
    {
        // Викликаємо метод створення через репозиторій
        var test = await _testRepository.CreateFromFlashcardsAsync(creatorId, flashcardIds, ct);
        return new TestDTO { TestId = test.TestId, CreatorId = creatorId };
    }

    public async Task<TestResultDTO> SubmitAsync(int testId, int userId, IReadOnlyList<(int flashcardId, string? userInput)> answers, CancellationToken ct)
    {
        var questionResults = new List<QuestionResult>();
        int correctCount = 0;

        // Перевіряємо кожну відповідь через репозиторій карток
        foreach (var ans in answers)
        {
            var card = await _flashcardRepository.GetAsync(ans.flashcardId, ct);
            bool isCorrect = false;

            if (card != null)
            {
                isCorrect = string.Equals(card.Answer.Trim(), ans.userInput?.Trim(), StringComparison.OrdinalIgnoreCase);
                if (isCorrect) correctCount++;
            }

            questionResults.Add(new QuestionResult
            {
                FlashcardId = ans.flashcardId,
                IsCorrect = isCorrect,
                UserInput = ans.userInput ?? ""
            });
        }

        decimal percent = answers.Count > 0 ? (decimal)correctCount / answers.Count * 100 : 0;
        int points = correctCount * 10; // Логіка нарахування балів для DTO

        var result = new TestResult
        {
            TestId = testId,
            UserId = userId,
            CorrectAnswersPercent = percent
            // PointsEarned тут немає, бо його немає в сутності бази даних
        };

        // Зберігаємо результати через репозиторій
        var savedResult = await _testResultRepository.AddAsync(result, questionResults, ct);

        return new TestResultDTO
        {
            TestResultId = savedResult.TestResultId,
            TestId = testId,
            UserId = userId,
            CorrectAnswersPercent = percent,
            PointsEarned = points,
            TestDate = DateTime.UtcNow
        };
    }
}