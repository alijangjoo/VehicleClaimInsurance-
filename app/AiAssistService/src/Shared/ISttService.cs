namespace Insurance.AiAssistService.Shared;

public interface ISttService
{
    Task<string> TranscribeAsync(string filePath);
}