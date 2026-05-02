using BrainBurst.Domain.Entities;

namespace BrainBurst.Application.Interfaces.Repositories;

public interface ITestRepository
{
    Task<Test> CreateFromFlashcardsAsync(int creatorId, IEnumerable<int> flashcardIds, CancellationToken ct);
    Task<Test?> GetAsync(int id, CancellationToken ct);
}