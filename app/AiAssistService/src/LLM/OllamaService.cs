namespace Insurance.AiAssistService.LLM;

using Insurance.AiAssistService.Domain;
using Insurance.AiAssistService.Shared;
using System.Net.Http.Json;
using System.Text.Json;

public class OllamaService : ILlmService
{
    private readonly HttpClient _httpClient;

    public OllamaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LlmResponse> ExtractAsync(string text)
    {
        var prompt = $@"
User sentence:
{text}

Available action:
{{
  ""login_by_credential"": {{
    ""userName"": ""string"",
    ""password"": ""string""
  }}
}}

Return ONLY valid JSON:
{{
  ""action"": ""..."",
  ""parameters"": {{ }}
}}";

        var request = new
        {
            //model = "qwen2.5:7b",
            model = "glm-4.6:cloud",
            prompt = prompt,
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:11434/api/generate",
            request);

        var raw = await response.Content.ReadFromJsonAsync<dynamic>();
        string json = raw.response;

        return JsonSerializer.Deserialize<LlmResponse>(json);
    }
}