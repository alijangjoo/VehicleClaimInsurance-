using Insurance.AiAssistService.AppServices;
using Insurance.AiAssistService.AppServices.ValidatorEngines;
using Insurance.AiAssistService.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.AiAssistService.Api.Controllers;

[ApiController]
[Route("api/voice")]
public class VoiceController : ControllerBase
{
    private readonly ISttService _stt;
    private readonly ILlmService _llm;
    private readonly ParameterValidator _validator;
    private readonly ActionRegistry _registry;

    public VoiceController(
        ISttService stt,
        ILlmService llm,
        ParameterValidator validator,
        ActionRegistry registry)
    {
        _stt = stt;
        _llm = llm;
        _validator = validator;
        _registry = registry;
    }

    [HttpPost("process")]
    public async Task<IActionResult> Process(IFormFile file)
    {
        var path = Path.GetTempFileName();

        using (var stream = System.IO.File.Create(path))
            await file.CopyToAsync(stream);

        var text = await _stt.TranscribeAsync(path);

        var llmResult = await _llm.ExtractAsync(text);

        var validation = _validator.Validate(llmResult);

        if (!validation.IsValid)
        {
            return BadRequest(new
            {
                status = "need_more_info",
                missingFields = validation.MissingFields
            });
        }

        var command = _registry.Get(llmResult.Action);
        var result = await command.ExecuteAsync(llmResult.Parameters);

        return Ok(result);
    }
}