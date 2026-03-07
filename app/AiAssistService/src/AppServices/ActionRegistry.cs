namespace Insurance.AiAssistService.AppServices;

using Insurance.AiAssistService.Domain;

public class ActionRegistry
{
    private readonly Dictionary<string, IActionCommand> _actions;

    public ActionRegistry(IEnumerable<IActionCommand> actions)
    {
        _actions = actions.ToDictionary(a => a.Name);
    }

    public IActionCommand Get(string name)
        => _actions[name];
}