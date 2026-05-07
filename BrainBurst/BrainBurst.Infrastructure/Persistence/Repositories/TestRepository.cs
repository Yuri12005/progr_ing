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
        // Додав Title, щоб база даних не сварилася на пусте поле
        var test = new Test
        {
            CreatorId = creatorId,
            Title = "Новий тест"
        };

        await _context.Tests.AddAsync(test, ct);
        await _context.SaveChangesAsync(ct);

        return test;
    }

    public async Task<Test?> GetAsync(int id, CancellationToken ct)
    {
        return await _context.Tests
            .Include(t => t.Creator)
            // === ДОДАНО: Щоб дістати колоду і всі її питання ===
            .Include(t => t.Tag)
                .ThenInclude(tag => tag.Flashcards)
            // ===================================================
            .FirstOrDefaultAsync(t => t.TestId == id, ct);
    }
}