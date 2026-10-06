using HackerNews.Api.Application.Options;
using HackerNews.Api.Application.Exceptions;
using HackerNews.Api.Domain.Interfaces;
using System.Text.Json;
using HackerNews.Api.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
namespace HackerNews.Api.Infrastructure;

public sealed class HistoryRepository(IHttpClientFactory clientFactory,IOptions<HackerNewsOptions> options) : IHistoryRepository, IDisposable
{
     private readonly MemoryCache cache = new(new MemoryCacheOptions { SizeLimit=1001 });
     private readonly SemaphoreSlim refreshGate = new(1,1);
     public async Task<List<History>> GetBestAsync(CancellationToken cancellationToken)
     {
        var ids = await GetCachedAsync<int[]>(options.Value.BestStoriesUrl,cancellationToken);
        return ids.Take(options.Value.MaxStories).Select(id=>new History { ID=id }).ToList();
     }
     public Task<Item> GetItemAsync(string itemUrl,CancellationToken cancellationToken) => GetCachedAsync<Item>(itemUrl,cancellationToken);
     private async Task<T> GetCachedAsync<T>(string url,CancellationToken cancellationToken) where T:class
     {
          if(cache.TryGetValue<T>(url,out var cached) && cached is not null) 
            return cached;
          await refreshGate.WaitAsync(cancellationToken);
          try
          {
            if(cache.TryGetValue<T>(url,out cached) && cached is not null) return cached;
            try
            {
                using var client = clientFactory.CreateClient("hackerNews");
                var value = await client.GetFromJsonAsync<T>(url,cancellationToken) ?? throw new UpstreamException("Upstream returned null.");
                cache.Set(url,value,new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow=TimeSpan.FromSeconds(options.Value.CacheSeconds),Size=1 });
                return value;
            }
            catch(Exception exception) when(exception is HttpRequestException or JsonException or Polly.Timeout.TimeoutRejectedException or Polly.CircuitBreaker.BrokenCircuitException or Polly.RateLimiting.RateLimiterRejectedException || exception is OperationCanceledException && !cancellationToken.IsCancellationRequested)
            { 
                throw new UpstreamException("Hacker News is temporarily unavailable.",exception); 
            }
          }
          finally { refreshGate.Release(); }
 }
 public void Dispose() 
    { 
        cache.Dispose(); refreshGate.Dispose(); 
    }
}
