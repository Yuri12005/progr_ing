using BrainBurst.Application.DTOs;
using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Application.Services;
using BrainBurst.Domain.Entities;
using Moq;
using Xunit;

namespace BrainBurst.Tests;

public class TestServiceTests
{
    [Fact]
    public async Task SubmitAsync_AllAnswersCorrect_Returns100PercentScore()
    {
        // 1. ARRANGE
        var mockTestRepo = new Mock<ITestRepository>();
        var mockResultRepo = new Mock<ITestResultRepository>();
        var mockFlashcardRepo = new Mock<IFlashcardRepository>();

        int testId = 1;
        int userId = 1;

        var fakeTest = new Test { TestId = testId, Title = "Математика" };
        var fakeFlashcard = new Flashcard { FlashcardId = 10, Question = "2+2", Answer = "4" };

        mockTestRepo.Setup(repo => repo.GetAsync(testId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(fakeTest);

        mockFlashcardRepo.Setup(repo => repo.GetAsync(10, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(fakeFlashcard);

        // ДОДАНО: Вчимо фейковий репозиторій повертати об'єкт при збереженні
        mockResultRepo.Setup(repo => repo.AddAsync(It.IsAny<TestResult>(), It.IsAny<IEnumerable<QuestionResult>>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new TestResult { TestResultId = 999 }); // 999 - просто вигаданий ID для тесту

        var testService = new TestService(mockTestRepo.Object, mockResultRepo.Object, mockFlashcardRepo.Object);

        var userAnswers = new List<(int flashcardId, string? userInput)>
        {
            (10, "4")
        };

        // 2. ACT
        var result = await testService.SubmitAsync(testId, userId, userAnswers, CancellationToken.None);

        // 3. ASSERT
        Assert.NotNull(result);
        Assert.Equal(100, result.CorrectAnswersPercent);
        Assert.Equal(999, result.TestResultId); // Перевіряємо, чи підтягнувся ID

        mockResultRepo.Verify(repo => repo.AddAsync(It.IsAny<TestResult>(), It.IsAny<IEnumerable<QuestionResult>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WrongAnswer_Returns0PercentScore()
    {
        // 1. ARRANGE
        var mockTestRepo = new Mock<ITestRepository>();
        var mockResultRepo = new Mock<ITestResultRepository>();
        var mockFlashcardRepo = new Mock<IFlashcardRepository>();

        int testId = 1;
        int userId = 1;

        var fakeTest = new Test { TestId = testId, Title = "Математика" };
        var fakeFlashcard = new Flashcard { FlashcardId = 10, Question = "2+2", Answer = "4" };

        mockTestRepo.Setup(repo => repo.GetAsync(testId, It.IsAny<CancellationToken>())).ReturnsAsync(fakeTest);
        mockFlashcardRepo.Setup(repo => repo.GetAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(fakeFlashcard);

        // ДОДАНО: Те саме для негативного тесту
        mockResultRepo.Setup(repo => repo.AddAsync(It.IsAny<TestResult>(), It.IsAny<IEnumerable<QuestionResult>>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new TestResult { TestResultId = 999 });

        var testService = new TestService(mockTestRepo.Object, mockResultRepo.Object, mockFlashcardRepo.Object);

        var userAnswers = new List<(int flashcardId, string? userInput)> { (10, "5") };

        // 2. ACT
        var result = await testService.SubmitAsync(testId, userId, userAnswers, CancellationToken.None);

        // 3. ASSERT
        Assert.NotNull(result);
        Assert.Equal(0, result.CorrectAnswersPercent);
    }

    // Сценарій 3: Частково правильні відповіді (50% успіху)
    [Fact]
    public async Task SubmitAsync_PartialCorrectAnswers_ReturnsCalculatedPercentAndPoints()
    {
        // 1. ARRANGE
        var mockTestRepo = new Mock<ITestRepository>();
        var mockResultRepo = new Mock<ITestResultRepository>();
        var mockFlashcardRepo = new Mock<IFlashcardRepository>();

        // Створюємо дві картки в базі
        var card1 = new Flashcard { FlashcardId = 1, Answer = "Правильно1" };
        var card2 = new Flashcard { FlashcardId = 2, Answer = "Правильно2" };

        mockFlashcardRepo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(card1);
        mockFlashcardRepo.Setup(r => r.GetAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(card2);

        mockResultRepo.Setup(repo => repo.AddAsync(It.IsAny<TestResult>(), It.IsAny<IEnumerable<QuestionResult>>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new TestResult { TestResultId = 42 });

        var testService = new TestService(mockTestRepo.Object, mockResultRepo.Object, mockFlashcardRepo.Object);

        // Юзер відповідає на перше правильно, на друге - з помилкою
        var userAnswers = new List<(int flashcardId, string? userInput)>
        {
            (1, "Правильно1"),
            (2, "Помилка")
        };

        // 2. ACT
        var result = await testService.SubmitAsync(1, 1, userAnswers, CancellationToken.None);

        // 3. ASSERT
        Assert.Equal(50, result.CorrectAnswersPercent); // 1 з 2 = 50%
        Assert.Equal(10, result.PointsEarned); // За 1 правильну дається 10 балів
    }

    // Сценарій 4: Граничний випадок (Edge Case) - Порожній список відповідей
    // (Перевіряємо, що програма не впаде від ділення на нуль)
    [Fact]
    public async Task SubmitAsync_EmptyAnswersList_Returns0Percent_DoesNotThrowException()
    {
        // 1. ARRANGE
        var mockTestRepo = new Mock<ITestRepository>();
        var mockResultRepo = new Mock<ITestResultRepository>();
        var mockFlashcardRepo = new Mock<IFlashcardRepository>();

        mockResultRepo.Setup(repo => repo.AddAsync(It.IsAny<TestResult>(), It.IsAny<IEnumerable<QuestionResult>>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new TestResult { TestResultId = 77 });

        var testService = new TestService(mockTestRepo.Object, mockResultRepo.Object, mockFlashcardRepo.Object);

        // Юзер відправив порожній тест
        var emptyAnswers = new List<(int flashcardId, string? userInput)>();

        // 2. ACT
        var result = await testService.SubmitAsync(1, 1, emptyAnswers, CancellationToken.None);

        // 3. ASSERT
        Assert.NotNull(result);
        Assert.Equal(0, result.CorrectAnswersPercent);
        Assert.Equal(0, result.PointsEarned);
    }

    // Сценарій 5: Негативний тест іншого методу (GetAsync)
    // Перевіряємо, чи правильно обробляється ситуація, коли тесту не існує
    [Fact]
    public async Task GetAsync_TestDoesNotExist_ReturnsNull()
    {
        // 1. ARRANGE
        var mockTestRepo = new Mock<ITestRepository>();
        var mockResultRepo = new Mock<ITestResultRepository>();
        var mockFlashcardRepo = new Mock<IFlashcardRepository>();

        // Кажемо репозиторію: "Коли тебе попросять тест з ID 999, поверни null"
        mockTestRepo.Setup(repo => repo.GetAsync(999, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Test)null);

        var testService = new TestService(mockTestRepo.Object, mockResultRepo.Object, mockFlashcardRepo.Object);

        // 2. ACT
        var result = await testService.GetAsync(999, CancellationToken.None);

        // 3. ASSERT
        Assert.Null(result); // Очікуємо, що сервіс теж поверне null, а не впаде з помилкою
    }
}