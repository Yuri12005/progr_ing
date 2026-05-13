using BrainBurst.Domain.Entities;

namespace BrainBurst.Application.Interfaces.Repositories;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>> GetAllWithCardsAsync(int userId, CancellationToken ct);
    Task<Tag?> GetByIdWithCardsAsync(int id, CancellationToken ct);
}