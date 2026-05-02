using BrainBurst.Application.DTOs;

namespace BrainBurst.Application.Interfaces.Services;

public interface IFlashcardService
{
    Task<FlashcardDTO> CreateAsync(int creatorId, string question, string answer, IEnumerable<string> tags, CancellationToken ct);
    Task<FlashcardDTO> UpdateAsync(int id, int editorId, string question, string answer, IEnumerable<string> tags, CancellationToken ct);
    Task DeleteAsync(int id, int requesterId, CancellationToken ct);
    Task<FlashcardDTO?> GetAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<FlashcardDTO>> ListAsync(int ownerId, string? search, CancellationToken ct);
}