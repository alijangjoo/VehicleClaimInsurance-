namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Configuration;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.RemoveFromCache;
using Microsoft.Extensions.Options;

public class RemoveFromCacheRequestValidator : AbstractValidator<RemoveFromCacheRequest>
{
    public RemoveFromCacheRequestValidator(IOptions<CacheOptions> options)
    {
        var cacheOptions = options.Value;

        RuleFor(c => c.Key)
            .NotEmpty()
            .WithMessage("Key cannot be empty");
    }
}