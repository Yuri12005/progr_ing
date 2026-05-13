using BrainBurst.Application.DTOs;
using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Application.Interfaces.Services;

namespace BrainBurst.Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository) => _tagRepository = tagRepository;

    public async Task<IReadOnlyList<TagDTO>> GetDecksAsync(int userId, CancellationToken ct)
    {
        // Передаємо userId у репозиторій
        var tags = await _tagRepository.GetAllWithCardsAsync(userId, ct);

        return tags.Select(t => new TagDTO
        {
            TagId = t.TagId,
            Name = t.Name,
            FlashcardsCount = t.Flashcards.Count,
            LastCardCreatedAt = t.Flashcards.Any() ? t.Flashcards.Max(f => f.CreatedAt) : null
        }).ToList();
    }

    public async Task<TagDTO?> GetDeckDetailsAsync(int id, CancellationToken ct)
    {
        var tag = await _tagRepository.GetByIdWithCardsAsync(id, ct);
        if (tag == null) return null;

        return new TagDTO
        {
            TagId = tag.TagId,
            Name = tag.Name,
            FlashcardsCount = tag.Flashcards.Count,
            // ДОДАЄМО МАПІНГ КАРТОК:
            Flashcards = tag.Flashcards.Select(f => new FlashcardDTO
            {
                FlashcardId = f.FlashcardId,
                Question = f.Question,
                Answer = f.Answer
            }).ToList()
        };
    }
}