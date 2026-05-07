using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Domain.Entities;
using BrainBurst.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BrainBurst.Infrastructure.Persistence.Repositories;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;
    public TagRepository(ApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<Tag>> GetAllWithCardsAsync(CancellationToken ct)
    {
        return await _context.Tags
            .Include(t => t.Flashcards)
            .ToListAsync(ct);
    }

    public async Task<Tag?> GetByIdWithCardsAsync(int id, CancellationToken ct)
    {
        return await _context.Tags
            .Include(t => t.Flashcards)
            .FirstOrDefaultAsync(t => t.TagId == id, ct);
    }
}