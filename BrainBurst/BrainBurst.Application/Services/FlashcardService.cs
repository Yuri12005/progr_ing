using BrainBurst.Application.DTOs;
using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Application.Interfaces.Services;
using BrainBurst.Domain.Entities;

namespace BrainBurst.Application.Services;

public class FlashcardService : IFlashcardService
{
    private readonly IFlashcardRepository _repository;

    public FlashcardService(IFlashcardRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlashcardDTO> CreateAsync(int creatorId, string question, string answer, IEnumerable<string> tags, CancellationToken ct)
    {
        var card = new Flashcard
        {
            Question = question,
            Answer = answer,
            CreatorId = creatorId
        };

        var saved = await _repository.AddAsync(card, tags, ct);

        return new FlashcardDTO
        {
            FlashcardId = saved.FlashcardId,
            Question = saved.Question,
            Answer = saved.Answer
        };
    }

    public async Task<FlashcardDTO?> GetAsync(int id, CancellationToken ct)
    {
        var f = await _repository.GetAsync(id, ct);
        if (f == null) return null;

        return new FlashcardDTO
        {
            FlashcardId = f.FlashcardId,
            Question = f.Question,
            Answer = f.Answer
        };
    }

    public async Task<FlashcardDTO> UpdateAsync(int id, int editorId, string question, string answer, IEnumerable<string> tags, CancellationToken ct)
    {
        var cardToUpdate = new Flashcard
        {
            FlashcardId = id,
            Question = question,
            Answer = answer,
            CreatorId = editorId
        };

        // Викликаємо метод оновлення в репозиторії
        await _repository.UpdateAsync(cardToUpdate, tags, ct);

        return new FlashcardDTO
        {
            FlashcardId = id,
            Question = question,
            Answer = answer
        };
    }

    public async Task DeleteAsync(int id, int requesterId, CancellationToken ct)
    {
        // Викликаємо метод видалення в репозиторії
        await _repository.DeleteAsync(id, requesterId, ct);
    }

    public async Task<IReadOnlyList<FlashcardDTO>> ListAsync(int ownerId, string? search, CancellationToken ct)
    {
        // Викликаємо метод пошуку в репозиторії
        var cards = await _repository.FindAsync(ownerId, search, ct);

        return cards.Select(f => new FlashcardDTO
        {
            FlashcardId = f.FlashcardId,
            Question = f.Question,
            Answer = f.Answer
        }).ToList();
    }
}