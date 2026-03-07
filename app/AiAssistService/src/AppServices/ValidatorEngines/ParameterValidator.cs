namespace Insurance.AiAssistService.AppServices.ValidatorEngines;

using Insurance.AiAssistService.Domain;
using System.Collections.Generic;

public class ParameterValidator
{
    public ValidationResult Validate(LlmResponse response)
    {
        var missing = new List<string>();

        if (!response.Parameters.ContainsKey("userName"))
            missing.Add("userName");

        if (!response.Parameters.ContainsKey("password"))
            missing.Add("password");

        return new ValidationResult
        {
            IsValid = missing.Count == 0,
            MissingFields = missing
        };
    }
}