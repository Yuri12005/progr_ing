using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BrainBurst.Application.DTOs;
using BrainBurst.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace BrainBurst.Infrastructure.ExternalServices;

public class OpenAiTestGenerationService : ITestGenerationService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiTestGenerationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new System.ArgumentNullException("OpenAI API Key is missing");
    }

    public async Task<IReadOnlyList<FlashcardDTO>> CreateFlashcardsFromTextAsync(int creatorId, string text, IEnumerable<string> tags, CancellationToken ct)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var systemPrompt = "Ти професійний викладач. З наданого тексту створи корисні флеш-картки (питання-відповідь). Поверни результат ВИКЛЮЧНО у форматі масиву JSON. Формат об'єкта: {\"Question\": \"...\", \"Answer\": \"...\"}. Ніякого іншого тексту.";

        var safeText = text.Length > 4000 ? text.Substring(0, 4000) : text;

        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = safeText }
            },
            temperature = 0.7
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent, ct);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync(ct);

        using var document = JsonDocument.Parse(responseString);
        var aiMessage = document.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        var cleanJson = aiMessage.Replace("```json", "").Replace("```", "").Trim();

        var flashcards = JsonSerializer.Deserialize<List<FlashcardDTO>>(cleanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // ДОДАНО: Прив'язуємо теги з параметрів до кожної згенерованої картки
        if (flashcards != null && tags != null)
        {
            var tagsList = tags.ToList();
            foreach (var card in flashcards)
            {
                card.Tags = tagsList;
            }
        }

        return flashcards ?? new List<FlashcardDTO>();
    }
}