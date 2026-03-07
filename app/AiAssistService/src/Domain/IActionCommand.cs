namespace Insurance.AiAssistService.Domain;

public interface IActionCommand
{
    string Name { get; }
    Task<ActionResult> ExecuteAsync(Dictionary<string, object> parameters);
}
