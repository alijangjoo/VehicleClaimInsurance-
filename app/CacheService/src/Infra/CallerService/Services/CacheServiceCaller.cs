namespace Insurance.CacheService.Infra.CallerService.Services;

using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models;
using Insurance.CacheService.Infra.CallerService.Models.Common;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.GetFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.RemoveFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class CacheServiceCaller : ICacheServiceCaller
{
    private readonly IOptions<CacheApiOptions> _cacheOptions;
    private readonly HttpService _httpService;
    private readonly ILogger<CacheServiceCaller> _logger;
    private string _baseURL = string.Empty;

    public CacheServiceCaller(
        IOptions<CacheApiOptions> cacheOptions,
        HttpService httpService,
        ILogger<CacheServiceCaller> logger)
    {
        _cacheOptions = cacheOptions;
        _httpService = httpService;
        _logger = logger;
        _baseURL = _cacheOptions.Value.BaseUrl;
    }

    public async Task<(GetFromCacheResponse Success, List<string> Error)> GetAsync(GetFromCacheRequest request)
    {
        _logger.LogInformation("Begin getAsync {@request}", request);
        var resopnse = await _httpService.PostAsync<GetFromCacheRequest, GetFromCacheResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/cache/get", request);

        _logger.LogInformation("End getAsync {@resopnse}", resopnse);

        return await Task.FromResult(resopnse!);
    }

    public async Task<(RemoveFromCacheResponse Success, List<string> Error)> RemoveAsync(RemoveFromCacheRequest request)
    {
        var resopnse = await _httpService.PostAsync<RemoveFromCacheRequest, RemoveFromCacheResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/cache/remove", request);

        return await Task.FromResult(resopnse);
    }

    public async Task<(SetToCacheResponse Success, List<string> Error)> SetAsync(SetToCacheRequest request)
    {
        var resopnse = await _httpService.PostAsync<SetToCacheRequest, SetToCacheResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/cache/set", request);

        return await Task.FromResult(resopnse!);
    }
}