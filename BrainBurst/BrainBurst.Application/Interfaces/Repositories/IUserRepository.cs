using BrainBurst.Domain.Entities;

namespace BrainBurst.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser> AddAsync(ApplicationUser user, CancellationToken ct);
    Task<ApplicationUser?> GetByIdAsync(int id, CancellationToken ct);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct);
    Task<IReadOnlyList<ApplicationUser>> GetAllAsync(CancellationToken ct);
    Task UpdateAsync(ApplicationUser user, CancellationToken ct);
}