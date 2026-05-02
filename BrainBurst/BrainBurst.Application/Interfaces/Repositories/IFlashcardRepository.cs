using BrainBurst.Domain.Entities;

namespace BrainBurst.Application.Interfaces.Repositories;

public interface IFlashcardRepository
{
    Task<Flashcard> AddAsync(Flashcard f, IEnumerable<string> tags, CancellationToken ct);
    Task UpdateAsync(Flashcard f, IEnumerable<string> tags, CancellationToken ct);
    Task DeleteAsync(int id, int ownerId, CancellationToken ct);
    Task<Flashcard?> GetAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Flashcard>> FindAsync(int ownerId, string? search, CancellationToken ct);
}