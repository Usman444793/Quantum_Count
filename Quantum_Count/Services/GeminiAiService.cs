using Google.GenAI;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
namespace Quantum_Count.Services;
public class GeminiAiService
{
    private readonly Google.GenAI.Client _client;
    private readonly string _model;
    public GeminiAiService(IConfiguration configuration)
    {
        var apiKey = configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Gemini API key is not configured in appsettings.json");
        }
        _model = configuration["Gemini:Model"] ?? "gemini-3.6-flash";
        _client = new Google.GenAI.Client(apiKey: apiKey);
    }
    public async Task<string> AskAsync(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return "Prompt cannot be empty.";
        }
        try
        {
            var response = await _client.Models.GenerateContentAsync( model: _model,contents: question);
            return response.Text ?? "No response was generated.";
        }
        catch (Exception ex)
        {
            return $"AI Service Error: {ex.Message}";
        }
    }
}
