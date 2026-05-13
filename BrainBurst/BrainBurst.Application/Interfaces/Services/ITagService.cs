using BrainBurst.Application.DTOs;

namespace BrainBurst.Application.Interfaces.Services;

public interface ITagService
{
    Task<IReadOnlyList<TagDTO>> GetDecksAsync(int userId, CancellationToken ct);
    Task<TagDTO?> GetDeckDetailsAsync(int id, CancellationToken ct);
}