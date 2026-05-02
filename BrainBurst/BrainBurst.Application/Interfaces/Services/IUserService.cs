using BrainBurst.Application.DTOs;

namespace BrainBurst.Application.Interfaces.Services;

public interface IUserService
{
    Task DeleteAccountAsync(int userId, CancellationToken ct);
    Task<UserDTO> GetAsync(int id, CancellationToken ct);
    Task<UserDTO> UpdateProfileAsync(int id, string fullName, CancellationToken ct);
    Task<IReadOnlyList<RankingEntryDTO>> GetLeaderboardAsync(int top, CancellationToken ct);
    Task ChangePasswordAsync(int userId, string oldPassword, string newPassword, CancellationToken ct);
}