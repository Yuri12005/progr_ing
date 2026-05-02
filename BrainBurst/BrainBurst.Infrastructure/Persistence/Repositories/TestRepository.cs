using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrainBurst.Infrastructure.Persistence.Repositories;

public class TestRepository : ITestRepository
{
    private readonly ApplicationDbContext _context;

    public TestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Test> CreateFromFlashcardsAsync(int creatorId, IEnumerable<int> flashcardIds, CancellationToken ct)
    {
        // Створюємо запис про новий тест
        var test = new Test
        {
            CreatorId = creatorId
        };

        await _context.Tests.AddAsync(test, ct);
        await _context.SaveChangesAsync(ct);
        
        return test;
    }

    public async Task<Test?> GetAsync(int id, CancellationToken ct)
    {
        return await _context.Tests
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(t => t.TestId == id, ct);
    }
}