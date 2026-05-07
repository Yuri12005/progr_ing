using BrainBurst.Domain.Entities;

namespace BrainBurst.Application.Interfaces.Repositories;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>> GetAllWithCardsAsync(CancellationToken ct);
    Task<Tag?> GetByIdWithCardsAsync(int id, CancellationToken ct);
}