using BrainBurst.Application.DTOs;

namespace BrainBurst.Application.Interfaces.Services;

public interface ITestGenerationService
{
    Task<IReadOnlyList<FlashcardDTO>> CreateFlashcardsFromTextAsync(int creatorId, string text, IEnumerable<string> tags, CancellationToken ct);
}