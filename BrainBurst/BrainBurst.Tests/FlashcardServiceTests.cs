using BrainBurst.Application.DTOs;
using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Application.Services;
using BrainBurst.Domain.Entities;
using Moq;
using Xunit;

namespace BrainBurst.Tests;

public class FlashcardServiceTests
{
    // Тест 1: Успішне створення картки та прив'язка її до колоди (тегів)
    [Fact]
    public async Task CreateAsync_ValidData_ReturnsCreatedFlashcardDTO()
    {
        // 1. ARRANGE (Підготовка)
        var mockRepo = new Mock<IFlashcardRepository>();

        int creatorId = 1;
        string question = "Що таке інкапсуляція?";
        string answer = "Приховування деталей реалізації";
        var tags = new List<string> { "ООП", "C#" }; // Імітуємо колоду

        // Вчимо фейковий репозиторій повертати збережену картку з ID
        mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Flashcard>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Flashcard f, IEnumerable<string> t, CancellationToken ct) =>
                {
                    f.FlashcardId = 105; // Імітуємо призначення ID базою даних
                    return f;
                });

        var service = new FlashcardService(mockRepo.Object);

        // 2. ACT (Дія)
        var result = await service.CreateAsync(creatorId, question, answer, tags, CancellationToken.None);

        // 3. ASSERT (Перевірка)
        Assert.NotNull(result);
        Assert.Equal(105, result.FlashcardId);
        Assert.Equal(question, result.Question);
        Assert.Equal(answer, result.Answer);

        // Перевіряємо, чи сервіс дійсно передав список колод (тегів) у репозиторій
        mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Flashcard>(), tags, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Тест 2: Успішне отримання існуючої картки
    [Fact]
    public async Task GetAsync_ExistingCard_ReturnsFlashcardDTO()
    {
        // 1. ARRANGE
        var mockRepo = new Mock<IFlashcardRepository>();
        var fakeCard = new Flashcard { FlashcardId = 5, Question = "2+2", Answer = "4" };

        mockRepo.Setup(repo => repo.GetAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeCard);

        var service = new FlashcardService(mockRepo.Object);

        // 2. ACT
        var result = await service.GetAsync(5, CancellationToken.None);

        // 3. ASSERT
        Assert.NotNull(result);
        Assert.Equal(5, result.FlashcardId);
        Assert.Equal("2+2", result.Question);
    }

    // Тест 3: Спроба отримати картку, якої не існує
    [Fact]
    public async Task GetAsync_NonExistingCard_ReturnsNull()
    {
        // 1. ARRANGE
        var mockRepo = new Mock<IFlashcardRepository>();

        // Якщо просять картку з ID 99, повертаємо null
        mockRepo.Setup(repo => repo.GetAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Flashcard?)null);

        var service = new FlashcardService(mockRepo.Object);

        // 2. ACT
        var result = await service.GetAsync(99, CancellationToken.None);

        // 3. ASSERT
        Assert.Null(result); // DTO має бути null, бо картки немає
    }

    // Тест 4: Видалення картки (перевіряємо, чи викликається метод репозиторію)
    [Fact]
    public async Task DeleteAsync_CallsRepositoryDeleteMethod()
    {
        // 1. ARRANGE
        var mockRepo = new Mock<IFlashcardRepository>();
        var service = new FlashcardService(mockRepo.Object);
        int cardId = 5;
        int requesterId = 1;

        // 2. ACT
        await service.DeleteAsync(cardId, requesterId, CancellationToken.None);

        // 3. ASSERT
        // Перевіряємо, що сервіс рівно 1 раз звернувся до репозиторію з проханням видалити картку
        mockRepo.Verify(repo => repo.DeleteAsync(cardId, requesterId, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Тест 5: Оновлення картки
    [Fact]
    public async Task UpdateAsync_CallsRepositoryUpdateMethod()
    {
        // 1. ARRANGE
        var mockRepo = new Mock<IFlashcardRepository>();
        var service = new FlashcardService(mockRepo.Object);

        int cardId = 10;
        int editorId = 1;
        string newQuestion = "Нове питання";
        string newAnswer = "Нова відповідь";
        var newTags = new List<string> { "Оновлений тег" };

        // 2. ACT
        await service.UpdateAsync(cardId, editorId, newQuestion, newAnswer, newTags, CancellationToken.None);

        // 3. ASSERT
        // Перевіряємо, що сервіс передав правильні дані в репозиторій
        mockRepo.Verify(repo => repo.UpdateAsync(
            It.Is<Flashcard>(f => f.FlashcardId == cardId && f.Question == newQuestion && f.Answer == newAnswer),
            newTags,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // Тест 6: Отримання списку карток користувача
    [Fact]
    public async Task ListAsync_ReturnsListOfFlashcardDTOs()
    {
        // 1. ARRANGE
        var mockRepo = new Mock<IFlashcardRepository>();

        var fakeCardsFromDb = new List<Flashcard>
        {
            new Flashcard { FlashcardId = 1, Question = "Питання 1", Answer = "Відповідь 1" },
            new Flashcard { FlashcardId = 2, Question = "Питання 2", Answer = "Відповідь 2" }
        };

        mockRepo.Setup(repo => repo.FindAsync(1, "Питання", It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeCardsFromDb);

        var service = new FlashcardService(mockRepo.Object);

        // 2. ACT
        var result = await service.ListAsync(1, "Питання", CancellationToken.None);

        // 3. ASSERT
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Питання 1", result[0].Question);
    }
}