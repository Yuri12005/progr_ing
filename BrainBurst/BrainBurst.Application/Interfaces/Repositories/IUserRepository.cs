using BrainBurst.Domain.Entities;

namespace BrainBurst.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task DeleteAsync(int id, CancellationToken ct);
    Task<User> GetByIdAsync(int id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User> AddAsync(User user, CancellationToken ct);
    Task UpdateAsync(User user, CancellationToken ct);
    Task<IReadOnlyList<User>> GetTopAsync(int take, CancellationToken ct);
}