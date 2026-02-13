namespace Insurance.CacheService.Infra.CallerService.Abstractions;

using Insurance.CacheService.Infra.CallerService.Models.Dtos.GetFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.RemoveFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;

public interface ICacheServiceCaller
{
    Task<(GetFromCacheResponse Success, List<string> Error)> GetAsync(GetFromCacheRequest request);
    Task<(SetToCacheResponse Success, List<string> Error)> SetAsync(SetToCacheRequest request);
    Task<(RemoveFromCacheResponse Success, List<string> Error)> RemoveAsync(RemoveFromCacheRequest request);
}