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

    public async Task<ApplicationUser> AddAsync(ApplicationUser user, CancellationToken ct)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
        return user;
    }

    public async Task<ApplicationUser?> GetByIdAsync(int id, CancellationToken ct)
    {
        // Зверни увагу: тепер ми використовуємо u.Id замість u.UserId
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Users.ToListAsync(ct);
    }

    public async Task UpdateAsync(ApplicationUser user, CancellationToken ct)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);
    }
}