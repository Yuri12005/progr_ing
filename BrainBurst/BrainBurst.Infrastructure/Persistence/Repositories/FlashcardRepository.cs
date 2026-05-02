using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrainBurst.Infrastructure.Persistence.Repositories;

public class FlashcardRepository : IFlashcardRepository
{
    private readonly ApplicationDbContext _context;

    public FlashcardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Flashcard> AddAsync(Flashcard f, IEnumerable<string> tags, CancellationToken ct)
    {
        // Додаємо або знаходимо існуючі теги
        foreach (var tagName in tags)
        {
            var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName, ct);
            if (existingTag != null)
            {
                f.Tags.Add(existingTag);
            }
            else
            {
                f.Tags.Add(new Tag { Name = tagName, CreatorId = f.CreatorId });
            }
        }

        await _context.Flashcards.AddAsync(f, ct);
        await _context.SaveChangesAsync(ct);
        return f;
    }

    public async Task DeleteAsync(int id, int ownerId, CancellationToken ct)
    {
        var flashcard = await _context.Flashcards.FirstOrDefaultAsync(f => f.FlashcardId == id && f.CreatorId == ownerId, ct);
        if (flashcard != null)
        {
            _context.Flashcards.Remove(flashcard);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<IReadOnlyList<Flashcard>> FindAsync(int ownerId, string? search, CancellationToken ct)
    {
        var query = _context.Flashcards
            .Include(f => f.Tags)
            .Where(f => f.CreatorId == ownerId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.ToLower();
            query = query.Where(f => f.Question.ToLower().Contains(searchTerm) || f.Answer.ToLower().Contains(searchTerm));
        }

        return await query.ToListAsync(ct);
    }

    public async Task<Flashcard?> GetAsync(int id, CancellationToken ct)
    {
        return await _context.Flashcards
            .Include(f => f.Tags)
            .FirstOrDefaultAsync(f => f.FlashcardId == id, ct);
    }

    public async Task UpdateAsync(Flashcard f, IEnumerable<string> tags, CancellationToken ct)
    {
        var existingFlashcard = await _context.Flashcards
            .Include(card => card.Tags)
            .FirstOrDefaultAsync(card => card.FlashcardId == f.FlashcardId, ct);

        if (existingFlashcard != null)
        {
            existingFlashcard.Question = f.Question;
            existingFlashcard.Answer = f.Answer;

            // Очищаємо старі теги та додаємо нові
            existingFlashcard.Tags.Clear();
            foreach (var tagName in tags)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName, ct) 
                          ?? new Tag { Name = tagName, CreatorId = f.CreatorId };
                existingFlashcard.Tags.Add(tag);
            }

            _context.Flashcards.Update(existingFlashcard);
            await _context.SaveChangesAsync(ct);
        }
    }
}