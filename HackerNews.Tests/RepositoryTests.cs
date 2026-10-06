using System.Net;
using System.Text;
using HackerNews.Api.Application.Exceptions;
using HackerNews.Api.Application.Options;
using HackerNews.Api.Infrastructure;
using Microsoft.Extensions.Options;
using Xunit;
namespace HackerNews.Tests;
public class RepositoryTests
{
 [Fact]
 public async Task ConcurrentMissesDownloadTheSameItemOnlyOnce()
 {
  using var handler=new CountingHandler(HttpStatusCode.OK);
  using var repository=new HistoryRepository(new TestClientFactory(handler),Options.Create(new HackerNewsOptions()));
  var tasks=Enumerable.Range(0,20).Select(_=>repository.GetItemAsync("https://example.test/1",CancellationToken.None));
  var results=await Task.WhenAll(tasks);
  Assert.Equal(1,handler.RequestCount);
  Assert.All(results,item=>Assert.Equal(1,item.Id));
 }
 [Fact]
 public async Task FailedDownloadsAreNotCached()
 {
  using var handler=new CountingHandler(HttpStatusCode.ServiceUnavailable);
  using var repository=new HistoryRepository(new TestClientFactory(handler),Options.Create(new HackerNewsOptions()));
  for(var attempt=0;attempt<2;attempt++) await Assert.ThrowsAsync<UpstreamException>(()=>repository.GetItemAsync("https://example.test/1",CancellationToken.None));
  Assert.Equal(2,handler.RequestCount);
 }
 private class TestClientFactory(HttpMessageHandler handler) : IHttpClientFactory
 {
  public HttpClient CreateClient(string name)=>new(handler,disposeHandler:false);
 }
 private class CountingHandler(HttpStatusCode statusCode) : HttpMessageHandler
 {
  private int requestCount;
  public int RequestCount=>requestCount;
  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
  {
   Interlocked.Increment(ref requestCount);
   await Task.Delay(10,cancellationToken);
   return new HttpResponseMessage(statusCode) {Content=new StringContent("{\"id\":1,\"type\":\"story\"}",Encoding.UTF8,"application/json")};
  }
 }
}
