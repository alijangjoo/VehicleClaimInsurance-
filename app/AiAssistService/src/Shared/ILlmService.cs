using Insurance.AiAssistService.Domain;

namespace Insurance.AiAssistService.Shared;

public interface ILlmService
{
    Task<LlmResponse> ExtractAsync(string text);
}
