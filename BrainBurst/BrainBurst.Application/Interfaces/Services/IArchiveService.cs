using BrainBurst.Application.DTOs;

namespace BrainBurst.Application.Interfaces.Services;

public interface IArchiveService
{
    Task<IReadOnlyList<ArchiveEntryDTO>> GetArchiveAsync(int userId, CancellationToken ct);
}