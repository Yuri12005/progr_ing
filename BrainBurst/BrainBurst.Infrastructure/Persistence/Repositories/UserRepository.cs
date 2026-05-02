using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrainBurst.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> AddAsync(User user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);
        return user;
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var user = await _context.Users.FindAsync(new object[] { id }, ct);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<User> GetByIdAsync(int id, CancellationToken ct)
    {
        var user = await _context.Users.FindAsync(new object[] { id }, ct);
        if (user == null) throw new Exception("User not found");
        return user;
    }

    public async Task<IReadOnlyList<User>> GetTopAsync(int take, CancellationToken ct)
    {
        return await _context.Users
            .OrderByDescending(u => u.Points)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);
    }
}