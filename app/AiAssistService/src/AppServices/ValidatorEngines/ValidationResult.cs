namespace Insurance.AiAssistService.AppServices.ValidatorEngines;

using System.Collections.Generic;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> MissingFields { get; set; }
}