namespace Insurance.AiAssistService.Domain;

public class LlmResponse
{
    public string Action { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}
