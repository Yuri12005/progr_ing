using System;
using System.Collections.Generic;

namespace BrainBurst.Application.DTOs;

public class TagDTO
{
    public int TagId { get; set; }
    public string Name { get; set; } = null!;
    public int FlashcardsCount { get; set; }
    public DateTime? LastCardCreatedAt { get; set; }

    // ДОДАЄМО ЦЕ, щоб передавати картки на фронтенд:
    public List<FlashcardDTO> Flashcards { get; set; } = new List<FlashcardDTO>();
}